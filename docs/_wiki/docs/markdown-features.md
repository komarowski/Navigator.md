# Markdown features

The project uses `Markdig` with a shared pipeline in `MarkdownManager`.

## Enabled features

- YAML front matter
- advanced markdown extensions
- custom link rendering
- custom tabs syntax

## Front matter usage

### Task pages

Task metadata is read into `TaskFrontMatter`:

```yaml
---
name: Add rebuild command
status: Open
---
```

Status values come from `TaskItemStatus`:

- `Open`
- `Frozen`
- `Closed`

### Q&A pages

Q&A metadata is read into `QaFrontMatter`:

```yaml
---
question: How does data.js get generated?
popularity: 2
---
```

## Title fallback

If front matter is missing, the scanners fall back to:

- first `# Heading`
- otherwise file name without extension

## Custom links

`CustomLinkRenderer` adds:

- `target="_blank"`
- `rel="noopener noreferrer"`

for non-image links.

## Custom tabs

Tabs are implemented as a custom Markdig block:

```markdown
!<tabs> sample
<tab> First
text
<tab> Second
text
</tabs>
```

The renderer turns this into a radio-button based tab UI.
