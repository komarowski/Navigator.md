using MarkdownNavigator.Infrastructure;
using MarkdownNavigator.Infrastructure.Services;

namespace MarkdownNavigator.Tests
{
  public class TestFixture : IDisposable
  {
    public string CurrentDirectory { get; private set; }

    public string TestDirectory { get; private set; }

    public TestFixture()
    {
      CurrentDirectory = Directory.GetCurrentDirectory();
      TestDirectory = Path.Combine(CurrentDirectory, FolderReservedNames.TestFolder);
      ResourceService.CopyResourceFiles(CurrentDirectory, FolderReservedNames.TestFolder);
    }

    public void Dispose()
    {
      if (Directory.Exists(TestDirectory))
      {
        Directory.Delete(TestDirectory, true);
      }
    }
  }
}
