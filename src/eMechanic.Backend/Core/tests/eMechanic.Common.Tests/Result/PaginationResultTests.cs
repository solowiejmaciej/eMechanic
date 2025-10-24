namespace eMechanic.Common.Tests.Result;

using eMechanic.Common.Result;

public class PaginationResultTests
{
    private sealed record TestEntity(int Id, string Name);
    private sealed record TestDto(int Id, string MappedName);

    [Theory]
    [InlineData(100, 10, 10)]
    [InlineData(101, 10, 11)]
    [InlineData(99, 10, 10)]
    [InlineData(0, 10, 0)]
    public void Constructor_Should_CalculateTotalPagesCorrectly(
        int totalCount, int pageSize, int expectedTotalPages)
    {
        // Arrange
        var items = new List<TestEntity>();

        // Act
        var paginationResult = new PaginationResult<TestEntity>(
            items, totalCount, 1, pageSize);

        // Assert
        Assert.Equal(expectedTotalPages, paginationResult.TotalPages);
    }

    [Fact]
    public void MapToDto_Should_CorrectlyMapAllItemsAndPreservePagination()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new TestEntity(1, "A"),
            new TestEntity(2, "B")
        };
        var paginationResult = new PaginationResult<TestEntity>(
            entities, 100, 2, 10);

        Func<TestEntity, TestDto> mapFunction =
            e => new TestDto(e.Id, e.Name.ToUpper(System.Globalization.CultureInfo.CurrentCulture));

        // Act
        var dtoResult = paginationResult.MapToDto(mapFunction);

        // Assert
        Assert.Equal(2, dtoResult.Items.Count());
        Assert.Equal("A", dtoResult.Items.First().MappedName);
        Assert.Equal("B", dtoResult.Items.Last().MappedName);

        Assert.Equal(100, dtoResult.TotalCount);
        Assert.Equal(2, dtoResult.PageNumber);
        Assert.Equal(10, dtoResult.PageSize);
        Assert.Equal(10, dtoResult.TotalPages);
    }
}
