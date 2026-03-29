---
question: How does the build start?
popularity: 2
---

# How does the build start?

The process starts in `MarkdownNavigator.Console/Program.cs`.

It builds a host, registers the required services, resolves `ConsoleCommandService`, and then calls `RunAsync()`.

The console service performs the initial refresh and starts the file watcher.
