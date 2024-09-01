using MarkdownNavigator.Domain.Services;
using MarkdownNavigator.Infrastructure.Services;

namespace MarkdownNavigator.Console.Services
{
  /// <summary>
  /// Service for processing console commands.
  /// </summary>
  /// <param name="fileWatcherService">Monitors changes to markdown files.</param>
  /// <param name="convertService">Service for converting markdown files to html.</param>
  public class ConsoleCommandService(IFileWatcherService fileWatcherService, IConvertService convertService)
  {
    private readonly IFileWatcherService fileWatcherService = fileWatcherService;
    private readonly IConvertService convertService = convertService;

    public void HandleConsoleCommands(CancellationTokenSource? cancellationToken = null)
    {
      ConsoleService.WriteGreeting();
      var updatedFilesNumber = convertService.ConvertAllHtml();
      ConsoleService.WriteLog($"Updated or added {updatedFilesNumber} files.", LogType.Info);
      while (true)
      {
        var input = ConsoleService.Prompt(">");

        if (EqualsAnyCommand(input, "r", "refresh"))
        {
          updatedFilesNumber = convertService.ConvertAllHtml(true);
          ConsoleService.WriteLog($"Updated or added {updatedFilesNumber} files.", LogType.Info);
          continue;
        }

        if (EqualsAnyCommand(input, "h", "hr", "hard refresh"))
        {
          updatedFilesNumber = convertService.ConvertAllHtml(true);
          ConsoleService.WriteLog($"Updated or added {updatedFilesNumber} files.", LogType.Info);
          continue;
        }

        if (EqualsAnyCommand(input, "w", "watch"))
        {
          fileWatcherService.StartFileWatcher();
          continue;
        }

        if (EqualsAnyCommand(input, "s", "stop"))
        {
          fileWatcherService.StopFileWatcher();
          continue;
        }

        if (EqualsAnyCommand(input, "e", "exit"))
        {
          cancellationToken?.Cancel();
          fileWatcherService.StopFileWatcher();
          break;
        }

        ConsoleService.WriteLog($"The \"{input}\" command does not exist.", LogType.Info);
      }
    }

    /// <summary>
    /// Checks the input string to match one of the commands.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <param name="commands">Command array.</param>
    /// <returns>True if the input is equal to at least one of the commands in the array.</returns>
    public static bool EqualsAnyCommand(string input, params string[] commands)
    {
      foreach (var command in commands)
      {
        if (string.Equals(input, command, StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }
      return false;
    }
  }
}
