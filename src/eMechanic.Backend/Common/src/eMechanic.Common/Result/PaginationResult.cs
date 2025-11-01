namespace eMechanic.Common.Result;

using System.Text.Json.Serialization;

public class PaginationResult<T>
{
    [JsonConstructor]
    private PaginationResult()
    {
    }

    public PaginationResult(IEnumerable<T> items, int count, int paginationParametersPageNumber,
        int paginationParametersPageSize)
    {
        Items = items;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)paginationParametersPageSize);
        PageNumber = paginationParametersPageNumber;
        PageSize = paginationParametersPageSize;
    }

    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PaginationResult<TDto> MapToDto<TDto>(Func<T, TDto> mapFunction)
    {
        return new PaginationResult<TDto>(
            Items.Select(mapFunction),
            TotalCount,
            PageNumber,
            PageSize
        );
    }
}
