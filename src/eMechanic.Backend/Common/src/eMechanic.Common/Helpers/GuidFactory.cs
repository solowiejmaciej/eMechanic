namespace eMechanic.Common.Helpers;

public static class GuidFactory
{
    public static Guid Create() => Guid.CreateVersion7();
}
