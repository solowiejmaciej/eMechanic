namespace eMechanic.Common.Result;

public class PaginationParameters : IPaginationParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SearchPhrase { get; set; }
}

public interface IPaginationParameters
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
    string? SearchPhrase { get; set; }
}
