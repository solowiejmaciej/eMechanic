namespace eMechanic.Events.Events.User;

public class UserUpdatedEvent : EventBase
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public UserUpdatedEvent(string email, string firstName, string lastName, Guid userId)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}
