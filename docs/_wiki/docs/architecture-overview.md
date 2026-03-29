# Architecture overview

`MarkdownNavigator` is split into three projects.

## 1. MarkdownNavigator.Console

This project provides the executable shell around the generator.

### Main responsibilities

- read configuration from `appsettings.json`
- configure dependency injection
- run a command loop with Spectre.Console
- watch the source folder for file changes
- trigger refresh or rebuild operations

### Main classes

- `Program` — bootstraps host, services, and logging
- `ConsoleCommandService` — command UI for `rebuild` and `exit`
- `FileWatcherService` — watches source changes and debounces build actions
- `SpectreConsoleLogger` / `SpectreConsoleLoggerProvider` — custom console logging

## 2. MarkdownNavigator.Core

This is the main application logic.

### Application layer

- `ISiteBuilder`
- `SiteBuilder`

`SiteBuilder` orchestrates wiki scanning, tasks scanning, Q&A scanning, and passes the result to the generator.

### Domain layer

Main domain pieces:

- entities: `Node`, `TreeStructure`, `TaskItem`, `QaItem`, `SiteContent`
- services: `TreeBuilder`, `TaskScanner`, `QaScanner`, `PathManager`
- markdown logic: `MarkdownManager`
- markdown extensions: custom links and custom tabs block

### Infrastructure layer

- `SiteGenerator`
- `EmbeddedResourceProvider`
- `HtmlTemplateGenerator`
- `MarkdownTemplateGenerator`

This layer writes files, templates pages, serializes navigation data, and extracts embedded static resources.

## 3. MarkdownNavigator.Tests

The tests are integration-focused and cover:

- resource extraction
- task/Q&A scanning
- site generation outputs

## Dependency direction

The executable depends on `Core`.
The tests depend on `Core`.
`Core` contains the reusable logic and embedded assets.
