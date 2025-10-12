using FluentAssertions;
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
}