namespace eMechanic.Application.Tests.Builders;

using eMechanic.Common.Result;

public class PaginationParametersBuilder
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    public PaginationParametersBuilder WithPageNumber(int pageNumber)
    {
        _pageNumber = pageNumber;
        return this;
    }

    public PaginationParametersBuilder WithPageSize(int pageSize)
    {
        _pageSize = pageSize;
        return this;
    }

    public PaginationParameters Build()
    {
        return new PaginationParameters { PageNumber = _pageNumber, PageSize = _pageSize };
    }
}
