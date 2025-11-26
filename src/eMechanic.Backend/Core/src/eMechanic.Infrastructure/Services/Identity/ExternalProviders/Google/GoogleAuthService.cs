namespace eMechanic.Infrastructure.Services.Identity.ExternalProviders.Google;

using Application.Users.Repositories;
using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Application.Users.Services;
using eMechanic.Common.Result;
using global::Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

internal sealed class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;

    private readonly UserManager<Infrastructure.Identity.Identity> _userManager;
    private readonly IUserService _userService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(UserManager<Infrastructure.Identity.Identity> userManager,
        IUserService userService,
        ITokenGenerator tokenGenerator,
        IConfiguration configuration,
        IUserRepository userRepository,
        ILogger<GoogleAuthService> logger)
    {
        _userManager = userManager;
        _userService = userService;
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _logger = logger;
        _clientId = configuration["Authentication:Google:ClientId"]
                    ?? throw new ArgumentNullException("Authentication:Google:ClientId is missing");
    }

    public async Task<Result<AuthenticatedIdentity, Error>> LoginAsync(string idToken, CancellationToken cancellationToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = [_clientId] };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var identityUser = await _userManager.FindByEmailAsync(payload.Email);

            if (identityUser is not null && identityUser!.Type == EIdentityType.Workshop)
            {
                return new Error(EErrorCode.NotImplementedError);
            }

            Guid userId;
            Guid identityId;

            if (identityUser is null)
            {
                var randomPassword = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

                var createResult = await _userService.CreateUserWithIdentityAsync(
                    payload.Email,
                    randomPassword,
                    payload.GivenName ?? "User",
                    payload.FamilyName ?? "Unknown",
                    providerName: "Google",
                    providerKey: payload.Subject,
                    cancellationToken: cancellationToken
                );

                if (!createResult.IsSuccess)
                {
                    return createResult.Error!;
                }

                (userId, identityId) = createResult.Value;
            }
            else
            {
                identityId = identityUser.Id;

                var logins = await _userManager.GetLoginsAsync(identityUser);
                if (logins.All(l => l.LoginProvider != "Google"))
                {
                    var addLoginResult = await _userManager.AddLoginAsync(identityUser, new UserLoginInfo("Google", payload.Subject, "Google"));
                    if (!addLoginResult.Succeeded)
                    {
                        return new Error(EErrorCode.ValidationError, "Could not link Google account to existing user.");
                    }
                }

                var user = await _userRepository.GetByIdentityIdAsync(identityId);
                if (user is null)
                {
                    return new Error(EErrorCode.InternalServerError);
                }
                userId = user.Id;
            }

            return new AuthenticatedIdentity(identityId, userId, payload.Email, EIdentityType.User);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an error while trying to sign in with google");
            return new Error(EErrorCode.ValidationError, "Provided token is invalid");
        }
    }
}
