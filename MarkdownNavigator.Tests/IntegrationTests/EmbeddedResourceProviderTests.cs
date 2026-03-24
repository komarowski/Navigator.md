using MarkdownNavigator.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MarkdownNavigator.Tests.IntegrationTests;

public class EmbeddedResourceProviderTests
{
    private EmbeddedResourceProvider _provider;
    private Mock<ILogger<EmbeddedResourceProvider>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<EmbeddedResourceProvider>>();
        _provider = new EmbeddedResourceProvider(_mockLogger.Object);
    }

    [Test]
    public void CopyPredefinedResources_WithValidTargetFolder_CreatesDirectoryAndExtractsResources()
    {
        // Arrange
        var targetFolder = Path.Combine(Path.GetTempPath(), $"test_resources_{Guid.NewGuid()}");
        
        try
        {
            // Act
            _provider.CopyPredefinedResources(targetFolder);

            // Assert
            Assert.That(Directory.Exists(targetFolder), Is.True, "Target folder should be created");
            var assetFolder = Path.Combine(targetFolder, "assets");
            Assert.That(Directory.Exists(assetFolder), Is.True, "Assets subdirectory should be created");
            
            var files = Directory.GetFiles(assetFolder, "*.*", SearchOption.AllDirectories);
            Assert.That(files.Length, Is.EqualTo(11), "Resources should be extracted to the target folder");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(targetFolder))
            {
                Directory.Delete(targetFolder, recursive: true);
            }
        }
    }
}
