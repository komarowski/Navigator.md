using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace MarkdownNavigator.Console.Logger;

/// <summary>
/// Custom logger provider that outputs to console using Spectre.Console for rich formatting.
/// </summary>
public class SpectreConsoleLoggerProvider : ILoggerProvider
{
    private readonly IAnsiConsole _console;

    /// <summary>
    /// Initializes a new instance with optional custom console.
    /// </summary>
    /// <param name="console">Custom IAnsiConsole instance, or null to use AnsiConsole.Console</param>
    public SpectreConsoleLoggerProvider(IAnsiConsole? console = null)
    {
        _console = console ?? AnsiConsole.Console;
    }

    /// <summary>
    /// Creates a logger for the specified category.
    /// </summary>
    public ILogger CreateLogger(string categoryName) =>
        new SpectreConsoleLogger(_console);

    /// <summary>
    /// Disposes provider resources.
    /// </summary>
    public void Dispose()
    {
        // No resources to dispose for console logging
    }
}
