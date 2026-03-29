---
question: How are markdown pages converted to HTML?
popularity: 2
---

# How are markdown pages converted to HTML?

`SiteGenerator.ConvertMarkdownToHtml(...)` reads markdown as UTF-8, sends it through `MarkdownManager.ConvertToHtml(...)`, wraps the result with `HtmlTemplateGenerator.WrapContent(...)`, and writes the final `.html` file.

It skips unchanged pages unless rebuild is forced.
