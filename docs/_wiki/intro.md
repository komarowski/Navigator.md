# MarkdownNavigator.md demo wiki

This wiki is a demo content set for the `MarkdownNavigator` repository. It is written in the same folder convention that the app expects, so it can be copied into a source folder and rendered by the console generator.

## What the project does

`MarkdownNavigator` converts local Markdown content into a static HTML knowledge base with:

- a wiki tree from `_wiki`
- a task list from `_tasks`
- a Q&A section from `_qa`
- generated `data.js` for client-side navigation
- embedded CSS, JS, fonts, and plugins copied into `assets`

## Solution structure

- `MarkdownNavigator.Console` — app entry point, DI, command loop, watcher, console logging
- `MarkdownNavigator.Core` — domain, markdown processing, site generation, templates, assets
- `MarkdownNavigator.Tests` — integration tests for scanners, generation, and resources

## Expected content folders

The generator uses these folders inside the configured source folder:

- `_wiki` — hierarchical documentation
- `_tasks` — flat or nested task notes with front matter
- `_qa` — flat or nested Q&A notes with front matter

## Main flow

1. `Program.cs` builds the host and registers services.
2. `ConsoleCommandService` runs the initial build and starts file watching.
3. `SiteBuilder` collects wiki, tasks, and Q&A data.
4. `SiteGenerator` converts markdown to HTML, writes `data.js`, copies assets, and generates `index.html`.

## Demo reading order

- [Architecture overview](docs/architecture-overview.html)
- [Build pipeline](docs/build-pipeline.html)
- [Markdown features](docs/markdown-features.html)
- [Frontend navigation](docs/frontend-navigation.html)
- [Testing](docs/testing.html)
