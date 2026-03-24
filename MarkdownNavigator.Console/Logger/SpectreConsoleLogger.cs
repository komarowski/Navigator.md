using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace MarkdownNavigator.Console.Logger;

/// <summary>
/// Spectre Console implementation of ILogger.
/// Outputs formatted log messages with color coding by log level.
/// </summary>
/// <param name="console">Spectre console for output</param>
public class SpectreConsoleLogger(IAnsiConsole console) : ILogger
{

    /// <summary>
    /// Begins a logical scope.
    /// </summary>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null; // Scope tracking not implemented
    }

    /// <summary>
    /// Checks if logging is enabled for the specified level.
    /// </summary>
    public bool IsEnabled(LogLevel logLevel)
    {
        return true; // Always enabled for simplicity; can be extended with configuration
    }

    /// <summary>
    /// Logs a message with format and exception handling.
    /// </summary>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);

        // Map log level to Spectre color
        var color = GetColorForLogLevel(logLevel);
        var levelName = logLevel.ToString();

        var output = $"[{color}]{Markup.Escape($"[{levelName}]")}[/] {Markup.Escape(message)}";

        console.MarkupLine(output);

        // Log exception details if present
        if (exception != null)
        {
            console.WriteException(exception, ExceptionFormats.ShortenEverything);
        }
    }

    /// <summary>
    /// Maps a LogLevel to a Spectre color.
    /// </summary>
    private static Color GetColorForLogLevel(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Critical => Color.Red,
            LogLevel.Error => Color.OrangeRed1,
            LogLevel.Warning => Color.Yellow,
            LogLevel.Information => Color.DarkCyan,
            LogLevel.Debug => Color.DarkCyan,
            LogLevel.Trace => Color.DarkCyan,
            _ => Color.White
        };
}