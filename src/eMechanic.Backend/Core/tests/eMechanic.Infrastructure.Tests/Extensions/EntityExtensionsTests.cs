namespace eMechanic.Infrastructure.Tests.Extensions;

using eMechanic.Domain.Repair.Enums;
using eMechanic.Common.Attributes;
using FluentAssertions;
using Repositories.Extensions;
using Xunit;

public class EntityExtensionsTests
{
    private sealed class TestEntity
    {
        public int Id { get; set; }

        [Searchable]
        public string Name { get; set; } = string.Empty;

        [Searchable]
        public string Description { get; set; } = string.Empty;

        public string SecretField { get; set; } = string.Empty;
    }

    private sealed class TestValueObject
    {
        public string Value { get; set; }
    }

    private sealed class ComplexTestEntity
    {
        [Searchable]
        public string? NullableString { get; set; }

        [Searchable]
        public TestValueObject? SearchableVo { get; set; }
    }

    private sealed class EnumEntity
    {
        [Searchable]
        public ERepairStatus Status { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    private readonly record struct TestRating(byte Value);

    private sealed class RatingEntity
    {
        [Searchable]
        public TestRating Rating { get; set; }
    }

    [Fact]
    public void ApplySearch_ShouldFilterBySearchPhrase_OnMarkedProperties()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new() { Id = 1, Name = "Audi A4", Description = "Nice car" },
            new() { Id = 2, Name = "BMW M3", Description = "Fast car" },
            new() { Id = 3, Name = "Fiat Punto", Description = "Small city car" }
        }.AsQueryable();

        // Act - szukamy "car" (jest w Description)
        var result = data.ApplySearch("car").ToList();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public void ApplySearch_ShouldFilter_WhenPhraseInName()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new() { Name = "Audi" },
            new() { Name = "BMW" }
        }.AsQueryable();

        // Act
        var result = data.ApplySearch("audi").ToList();

        // Assert
        result.Should().ContainSingle(x => x.Name == "Audi");
    }

    [Fact]
    public void ApplySearch_ShouldIgnore_UnmarkedProperties()
    {
        // Arrange
        var data = new List<TestEntity>
        {
            new() { Name = "Test", SecretField = "HiddenValue" }
        }.AsQueryable();

        // Act
        var result = data.ApplySearch("HiddenValue").ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ApplySearch_ShouldReturnAll_WhenPhraseIsEmpty()
    {
        var data = new List<TestEntity> { new() { Name = "A" } }.AsQueryable();
        var result = data.ApplySearch("").ToList();
        result.Should().HaveCount(1);
    }

    [Fact]
    public void ApplySearch_ShouldHandle_NullableProperties()
    {
        // Arrange
        var data = new List<ComplexTestEntity>
        {
            new() { NullableString = "findme" },
            new() { NullableString = null }
        }.AsQueryable();

        // Act
        var result = data.ApplySearch("findme").ToList();

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public void ApplySearch_ShouldWorkWithValueObjects()
    {
        // Arrange
        var data = new List<ComplexTestEntity>
        {
            new() { SearchableVo = new TestValueObject { Value = "findme" } },
            new() { SearchableVo = new TestValueObject { Value = "ignore" } },
            new() { SearchableVo = null }
        }.AsQueryable();

        // Act
        var result = data.ApplySearch("findme").ToList();

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public void ApplySearch_ShouldFilterByEnumStatus_WhenSearchPhraseMatchesEnumName()
    {
        var data = new List<EnumEntity>
        {
            new() { Status = ERepairStatus.Scheduled },
            new() { Status = ERepairStatus.Paid }
        }.AsQueryable();

        var result = data.ApplySearch("paid").ToList();

        result.Should().ContainSingle(x => x.Status == ERepairStatus.Paid);
    }

    [Fact]
    public void ApplySearch_ShouldReturnAll_WhenEnumSearchPhraseIsInvalid()
    {
        var data = new List<EnumEntity>
        {
            new() { Status = ERepairStatus.Scheduled },
            new() { Status = ERepairStatus.Paid }
        }.AsQueryable();

        var result = data.ApplySearch("not-a-status").ToList();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplySearch_ShouldFilterByNumericValueObject_WhenSearchPhraseIsNumeric()
    {
        var data = new List<RatingEntity>
        {
            new() { Rating = new TestRating(3) },
            new() { Rating = new TestRating(5) }
        }.AsQueryable();

        var result = data.ApplySearch("5").ToList();

        result.Should().ContainSingle(x => x.Rating.Value == 5);
    }

    [Fact]
    public void ApplySearch_ShouldReturnAll_WhenNumericValueObjectSearchPhraseIsInvalid()
    {
        var data = new List<RatingEntity>
        {
            new() { Rating = new TestRating(3) },
            new() { Rating = new TestRating(5) }
        }.AsQueryable();

        var result = data.ApplySearch("five").ToList();

        result.Should().HaveCount(2);
    }
}
