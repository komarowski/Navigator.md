# Extension points

This project is already structured so new features can be added without rewriting the whole generator.

## Good places to extend

### New markdown syntax

Add a new Markdig extension beside:

- `CustomLinkExtension`
- `TabsExtension`

Then register it in `MarkdownManager`.

### New metadata types

Add front matter models and scanners similar to:

- `TaskFrontMatter`
- `QaFrontMatter`
- `TaskScanner`
- `QaScanner`

### New UI plugins

Add embedded resources and register them in:

- `EmbeddedResourceProvider.Resources`
- `HtmlTemplateGenerator.WrapContent(...)`

### New commands

Extend `ConsoleCommandService.CommandItems` and add a matching command case.

## Improvement ideas

- persist build state instead of using only file timestamps
- add search index generation
- add tag filtering for tasks and Q&A
- split `core.js` into smaller frontend modules
- add unit tests for markdown parsing edge cases
