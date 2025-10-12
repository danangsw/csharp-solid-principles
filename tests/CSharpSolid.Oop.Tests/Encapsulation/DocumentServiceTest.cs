using Xunit;
using FluentAssertions;
using System;
using System.Text;
using CSharpSolid.Oop.Encapsulation;

namespace CSharpSolid.Oop.Tests.Encapsulation;


public class DocumentServiceTest
{
    private const string ValidUserId = "user123";
    private const string ValidTitle = "Test Document";
    private static readonly byte[] ValidContent = System.Text.Encoding.UTF8.GetBytes("Test content");
    private const string ValidFileName = "test.txt";

    [Fact]
    public void Constructor_ShouldInitializeDocumentCorrectly()
    {
        // Arrange & Act
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);

        // Assert
        document.Title.Should().Be(ValidTitle);
        document.FileName.Should().Be(ValidFileName);
        document.CreatedBy.Should().Be(ValidUserId);
        document.AccessLevel.Should().Be(AccessLevel.Public);
        document.Status.Should().Be(DocumentStatus.Draft);
        document.SizeInBytes.Should().Be(ValidContent.Length);
        document.Versions.Should().HaveCount(1);
        document.Versions[0].VersionNumber.Should().Be(1);
        document.Tags.Should().HaveCount(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void Constructor_ShouldThrowException_WhenTitleIsInvalid(string invalidTitle)
    {
        // Arrange & Act & Assert
        var act = () => new DocumentService(invalidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        act.Should().Throw<ArgumentException>().WithMessage("*Title*");
    }

    [Theory]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("My<File>Name/.txt")]
    public void Constructor_InvalidFileName_ShouldThrowException(string invalidFileName)
    {
        // Arrange & Act & Assert
        var act = () => new DocumentService(ValidTitle, ValidContent, invalidFileName, ValidUserId, AccessLevel.Public);
        act.Should().Throw<ArgumentException>().WithMessage("*File name*");
    }

    [Fact]
    public void Constructor_InvalidContent_ShouldThrowException()
    {
        // Arrange & Act & Assert
        var act = () => new DocumentService(ValidTitle, null, ValidFileName, ValidUserId, AccessLevel.Public);
        act.Should().Throw<ArgumentException>().WithMessage("*Content*");
    }

    [Fact]
    public void Constructor_WithTooLargeContent_ShouldThrowException()
    {
        // Arrange
        var largeContent = new byte[51 * 1024 * 1024];

        // Act & Assert
        var act = () => new DocumentService(ValidTitle, largeContent, ValidFileName, ValidUserId, AccessLevel.Public);
        act.Should().Throw<ArgumentException>().WithMessage("*Content size*");
    }

    [Fact]
    public void ReadContent_ShouldReturnContent()
    {
        // Arrange & Act
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        var expectedContent = Encoding.UTF8.GetString(ValidContent);
        var actualContent = Encoding.UTF8.GetString(document.GetContent(document.DocumentId, ValidUserId));

        // Assert
        actualContent.Should().Be(expectedContent);
        actualContent.Should().NotBeSameAs(expectedContent);
    }

    [Fact]
    public void UpdateContent_ShouldUpdateContentAndCreateNewVersion()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        var newContent = Encoding.UTF8.GetBytes("Updated content");

        // Act
        document.UpdateContent(newContent, ValidUserId);

        // Assert
        document.SizeInBytes.Should().Be(newContent.Length);
        document.Versions.Should().HaveCount(2);
        document.Versions[1].VersionNumber.Should().Be(2);
        document.Versions[1].Changes.Should().Contain("updated");
        document.ModifiedBy.Should().Be(ValidUserId);
        document.ModifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void UpdateContent_ReadOnlyAccess_ShouldThrowException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.ReadOnly);
        var newContent = Encoding.UTF8.GetBytes("Updated content");

        // Act & Assert
        var act = () => document.UpdateContent(newContent, ValidUserId);
        act.Should().Throw<UnauthorizedAccessException>().WithMessage("*edit*");
    }

    [Fact]
    public void UpdateContent_ArchivedDocument_ShouldThrowException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        document.Archive(ValidUserId);
        var newContent = Encoding.UTF8.GetBytes("Updated content");

        // Act & Assert
        var act = () => document.UpdateContent(newContent, ValidUserId);
        act.Should().Throw<InvalidOperationException>().WithMessage("*edit*");
    }

    [Fact]
    public void Publish_ShouldChangeStatusAndCreateNewVersion()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);

        // Act
        Action act = () => document.Publish(ValidUserId);

        // Assert
        act.Should().NotThrow();
        document.Status.Should().Be(DocumentStatus.Published);
        document.Versions.Should().HaveCount(2);
        document.Versions[1].VersionNumber.Should().Be(2);
    }

    [Fact]
    public void Publish_AlreadyPublished_ShouldThrowException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        document.Publish(ValidUserId);

        // Act
        Action act = () => document.Publish(ValidUserId);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*already published*");
    }

    [Fact]
    public void Archive_ShouldChangeStatusAndCreateNewVersion()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        document.Publish(ValidUserId);

        // Act
        Action act = () => document.Archive(ValidUserId);

        // Assert
        act.Should().NotThrow();
        document.CreatedBy.Should().Be(ValidUserId);
        document.ModifiedBy.Should().Be(ValidUserId);
        document.ModifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        document.Status.Should().Be(DocumentStatus.Archived);
        document.Versions.Should().HaveCount(3);
        document.Versions[2].VersionNumber.Should().Be(3);
        document.Versions[2].Changes.Should().Contain("archived");
    }

    [Fact]
    public void Archive_AlreadyArchived_ShouldThrowException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        document.Publish(ValidUserId);
        document.Archive(ValidUserId);

        // Act
        Action act = () => document.Archive(ValidUserId);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*already archived*");
        document.Status.Should().Be(DocumentStatus.Archived);
        document.Versions.Should().HaveCount(3);
    }

    [Fact]
    public void GetContent_ValidUser_ShouldReturnContent()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);

        // Act
        byte[] content = document.GetContent(document.DocumentId, ValidUserId);
        Action act = () => document.GetContent(document.DocumentId, ValidUserId);

        // Assert
        act.Should().NotThrow();
        content.Should().BeEquivalentTo(ValidContent);
        content.Should().NotBeSameAs(ValidContent);
    }

    [Fact]
    public void GetContent_RestrictedAccessByDifferentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Restricted);

        // Act
        Action act = () => document.GetContent(document.DocumentId, "differentUser");

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("You do not have permission to access this document.*");
    }

    [Fact]
    public void AddTag_ValidTag_AddsTag()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);

        // Act
        document.AddTag("important", ValidUserId);

        // Assert
        document.Tags.Should().Contain("important");
    }

    [Fact]
    public void AddTag_DuplicateTag_ThrowsInvalidOperationException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        document.AddTag("important", ValidUserId);

        // Act
        Action act = () => document.AddTag("important", ValidUserId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already exists.*");
    }

    [Fact]
    public void RemoveTag_ExistingTag_RemovesTag()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);
        document.AddTag("important", ValidUserId);

        // Act
        document.RemoveTag("important", ValidUserId);

        // Assert
        document.Tags.Should().NotContain("important");
    }

    [Fact]
    public void RemoveTag_NonExistingTag_ThrowsInvalidOperationException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);

        // Act
        Action act = () => document.RemoveTag("nonexistent", ValidUserId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*does not exist.*");
    }

    [Fact]
    public void AccessLevel_InvalidValue_ThrowsArgumentException()
    {
        // Arrange
        var document = new DocumentService(ValidTitle, ValidContent, ValidFileName, ValidUserId, AccessLevel.Public);

        // Act
        Action act = () => document.AccessLevel = (AccessLevel)999;

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid access level.*");
    }
}
