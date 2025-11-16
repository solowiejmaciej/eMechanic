namespace eMechanic.Infrastructure.Tests.Builders;

using System;
using System.IO;
using FluentAssertions;
using Storage.Builders;

public class VehicleDocumentPathBuilderTests
{
    private readonly VehicleDocumentPathBuilder _pathBuilder;

    public VehicleDocumentPathBuilderTests()
    {
        _pathBuilder = new VehicleDocumentPathBuilder();
    }

    [Fact]
    public void GetVehicleDirectoryPath_ShouldReturnCorrectPath()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var expectedPath = $"vehicle-documents/{vehicleId}/";

        // Act
        var result = _pathBuilder.GetVehicleDirectoryPath(vehicleId);

        // Assert
        result.Should().Be(expectedPath);
    }

    [Theory]
    [InlineData("faktura.pdf", ".pdf")]
    [InlineData("zdjecie.jpg", ".jpg")]
    [InlineData("skan.rejestracyjny.png", ".png")]
    [InlineData("plikBezRozszerzenia", "")]
    public void BuildNewDocumentPath_ShouldReturnCorrectPathWithExtension(string originalFileName, string expectedExtension)
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var newDocumentId = Guid.NewGuid();
        var expectedPath = $"vehicle-documents/{vehicleId}/{newDocumentId}{expectedExtension}";

        // Act
        var result = _pathBuilder.BuildNewDocumentPath(vehicleId, newDocumentId, originalFileName);

        // Assert
        result.Should().Be(expectedPath);
        Path.GetExtension(result).Should().Be(expectedExtension);
    }
}
