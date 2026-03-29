# Testing

The test project is `MarkdownNavigator.Tests`.

## Style

The current tests are mostly integration tests. They build real folders in temporary paths and verify produced files.

## Covered areas

### EmbeddedResourceProviderTests

Verifies that:

- the target folder is created
- the `assets` subfolder is created
- embedded files are extracted

### QaScannerTests

Verifies that:

- markdown files are found recursively
- front matter is read
- fallback title logic works
- invalid or missing front matter does not break scanning

### SiteBuilderTests

Verifies that:

- build does not fail when source folder is missing
- expected output HTML and `data.js` are generated
- generated site structure matches a realistic input set

## Good extension points

Useful next tests:

- `TaskScanner` sort order and fallback title behavior
- `TreeBuilder` handling of missing `index.md`
- `SiteGenerator` incremental build skipping
- `MarkdownManager` custom tabs rendering
- `PathManager` relative path conversion
