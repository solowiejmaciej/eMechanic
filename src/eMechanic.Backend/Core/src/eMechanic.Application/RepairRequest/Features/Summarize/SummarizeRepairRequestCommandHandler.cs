namespace eMechanic.Application.RepairRequest.Features.Summarize;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;
using Services;

public class SummarizeRepairRequestCommandHandler : IResultCommandHandler<SummarizeRepairRequestCommand, string>
{
    private readonly IRepairRequestSummaryService _repairRequestSummaryService;
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IUserContext _userContext;

    public SummarizeRepairRequestCommandHandler(
        IRepairRequestSummaryService repairRequestSummaryService,
        IRepairRequestRepository repairRequestRepository,
        IUserContext userContext)
    {
        _repairRequestSummaryService = repairRequestSummaryService;
        _repairRequestRepository = repairRequestRepository;
        _userContext = userContext;
    }

    public async Task<Result<string, Error>> Handle(SummarizeRepairRequestCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var repairRequestResult = await _repairRequestRepository.GetForUserByIdAsync(userId, request.RepairRequestId, cancellationToken);

        if (repairRequestResult == null)
        {
            return new Error(EErrorCode.NotFoundError, "Repair request not found.");
        }

        var summaryReport = await _repairRequestSummaryService.GenerateSummaryReport(repairRequestResult, cancellationToken);

        repairRequestResult.SetSummaryReport(summaryReport);

        await _repairRequestRepository.SaveChangesAsync(cancellationToken);
        return summaryReport;
    }
}
