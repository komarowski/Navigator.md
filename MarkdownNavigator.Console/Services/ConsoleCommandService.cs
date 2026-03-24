using MarkdownNavigator.Core.Application;
using Spectre.Console;

namespace MarkdownNavigator.Console.Services;

/// <summary>
/// Implementation of console command service.
/// Orchestrates the build process and handles file watching.
/// </summary>
public class ConsoleCommandService(
    ISiteBuilder siteBuilder,
    FileWatcherService fileWatcherService)
{
    private static readonly CommandItem[] CommandItems =
    [
        new("rebuild", "b", "Rebuild all HTML files from scratch"),
        new("exit", "e", "Exit program")
    ];

    /// <summary>
    /// Runs the console workflow:
    /// initial refresh, start watcher, and process commands.
    /// </summary>
    public Task RunAsync()
    {
        WriteCommandsTable();

        RunRefresh("Initial refresh...");
        fileWatcherService.Start();

        AnsiConsole.MarkupLine("[grey]Watching source folder for changes...[/]");
        AnsiConsole.WriteLine();

        while (true)
        {
            var command = ReadCommand();

            switch (command)
            {
                case "rebuild":
                case "b":
                    RunRebuild("Manual rebuild...");
                    break;

                case "exit":
                case "e":
                    AnsiConsole.MarkupLine("[yellow]Closing Navigator.md...[/]");
                    return Task.CompletedTask;

                default:
                    AnsiConsole.MarkupLine("[red]Unknown command.[/] Use [cyan]rebuild[/] ([cyan]b[/]) or [cyan]exit[/] ([cyan]e[/]).");
                    break;
            }

            AnsiConsole.WriteLine();
        }
    }

    /// <summary>
    /// Renders the list of available commands.
    /// </summary>
    private static void WriteCommandsTable()
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine();

        AnsiConsole.Write(
            new Rule("[cyan1 bold]Welcome to Navigator.md[/]")
                .LeftJustified()
                .RuleStyle("cyan1"));

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[grey]Tip:[/] type the full command or use its short alias shown in [cyan]( )[/].");

        AnsiConsole.Write(CreateCommandsTable());
        AnsiConsole.WriteLine();
    }

    private static Table CreateCommandsTable()
    {
        var table = new Table
        {
            Border = TableBorder.Rounded,
            Expand = true
        };

        table.BorderColor(Color.Grey);
        table.AddColumn(new TableColumn("[bold]Command[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold]Alias[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Description[/]").LeftAligned());

        foreach (var command in CommandItems)
        {
            table.AddRow(
                $"[darkcyan bold]{command.Name}[/]",
                $"[yellow]({command.Alias})[/]",
                $"[silver]{command.Description}[/]");
        }

        return table;
    }

    private static string ReadCommand()
    {
        return AnsiConsole
            .Prompt(new TextPrompt<string>("[cyan]Command[/]:"))
            .Trim()
            .ToLowerInvariant();
    }

    private void RunRefresh(string message)
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start(message, _ =>
            {
                siteBuilder.Build();
            });

        AnsiConsole.MarkupLine("[green]Refresh completed.[/]");
    }

    private void RunRebuild(string message)
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start(message, _ =>
            {
                siteBuilder.Build(forceRebuild: true);
            });

        AnsiConsole.MarkupLine("[green]Rebuild completed.[/]");
    }

    private sealed record CommandItem(string Name, string Alias, string Description);
}