---
question: How are assets deployed into the output folder?
popularity: 1
---

# How are assets deployed into the output folder?

`EmbeddedResourceProvider.CopyPredefinedResources(...)` extracts embedded files from the assembly into the source folder.

This includes:

- `core.css`
- `core.js`
- fonts
- Prism plugin files
- slider plugin files
- copy-code plugin files
