# Build pipeline

This page describes the end-to-end generation flow.

## Entry point

`Program.cs` configures services and resolves `ConsoleCommandService`.

## Runtime workflow

`ConsoleCommandService.RunAsync()` does the following:

1. prints the available commands
2. runs an initial refresh
3. starts the file watcher
4. waits for console commands

## SiteBuilder pipeline

`SiteBuilder.Build()`:

1. verifies that the configured source folder exists
2. builds the wiki tree from `_wiki`
3. scans `_tasks`
4. scans `_qa`
5. creates a `SiteContent` aggregate
6. calls `ISiteGenerator.GenerateWeb(...)`

## SiteGenerator pipeline

`SiteGenerator.GenerateWeb(...)` performs these steps:

1. create missing `index.md` files for wiki folders without one
2. convert wiki markdown to HTML
3. convert task markdown to HTML
4. convert Q&A markdown to HTML
5. generate `data.js`
6. copy embedded assets into `assets`
7. generate the root `index.html`

## Incremental behavior

`ConvertMarkdownToHtml(...)` skips unchanged pages unless:

- the HTML file does not exist
- the markdown file is newer than the HTML file
- `forceRebuild` is `true`

## Watcher behavior

`FileWatcherService` debounces multiple file system events and chooses the stronger action:

- markdown content change → refresh
- create, delete, rename, or watcher error → rebuild
