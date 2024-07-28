using Spectre.Console;

namespace MarkdownNavigator.Infrastructure.Services
{
  /// <summary>
  /// Type of log records.
  /// </summary>
  public enum LogType
  {
    Info,
    Warning,
    Error
  }

  /// <summary>
  /// Service for displaying console application feedback.
  /// </summary>
  public static class ConsoleService
  {
    /// <summary>
    /// Write color text.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="color">Text color.</param>
    public static void WriteColor(string text, ConsoleColor color)
    {
      Console.ForegroundColor = color;
      Console.Write(text);
      Console.ForegroundColor = ConsoleColor.White;
    }

    /// <summary>
    /// Write greeting.
    /// </summary>
    public static void WriteGreeting()
    {
      var welcomeString = new Padder(new Markup("[cyan3 bold underline]Welcome to Navigator.md![/]")).PadRight(30);
      AnsiConsole.Write(welcomeString);
      AnsiConsole.Write(new Rule("Commands:").LeftJustified());
      var rows = new List<Markup>()
      {
        new("[darkcyan bold] refresh (r)[/] - force refresh all html files"),
        new("[darkcyan bold] watch (w)[/] - start FileSystemWatcher"),
        new("[darkcyan bold] stop (s)[/] - stop FileSystemWatcher"),
        new("[darkcyan bold] exit (e)[/] - exit program")
      };
      AnsiConsole.Write(new Rows(rows));
      AnsiConsole.Write(new Rule());
      AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Write log.
    /// </summary>
    /// <param name="text">Log text.</param>
    /// <param name="logType">Log type.</param>
    public static void WriteLog(string text, LogType logType)
    {
      AnsiConsole.Markup("[silver]LOG: [/]");
      var log = GetLogString(text, logType);
      AnsiConsole.Markup(log);
      AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Write log before ReadLine string.
    /// </summary>
    /// <param name="text">Log text.</param>
    /// <param name="logType">Log type.</param>
    /// <param name="readLineText">ReadLine text.</param>
    public static void WriteLogBeforeReadLine(string text, LogType logType, string readLineText)
    {
      Console.Write("\r");
      WriteLog(text, logType);
      Console.Write($"\r{readLineText}");
    }

    /// <summary>
    /// Get the colored text of the log.
    /// </summary>
    /// <param name="text">Log text.</param>
    /// <param name="logType">Log type.</param>
    /// <returns>Colored text.</returns>
    private static string GetLogString(string text, LogType logType) => (logType) switch
    {
      (LogType.Warning) => $"[gold3_1]{text}[/]",
      (LogType.Error) => $"[red3]{text}[/]",
      (LogType.Info) or _ => text
    };

    public static string Prompt(string text)
    {
      return AnsiConsole.Prompt(new TextPrompt<string>(text).AllowEmpty());
    }
  }
}
