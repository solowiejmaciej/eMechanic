namespace eMechanic.Common.DDD;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}
