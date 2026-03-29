# Frontend navigation

The generated HTML shell uses:

- `assets/core.css`
- `assets/core.js`
- syntax highlighting and utility plugins

## Main page layout

`HtmlTemplateGenerator.WrapContent(...)` creates a standard page with:

- header tabs: Wiki / Tasks / Q&A
- sidebar
- main markdown article
- table of contents area

## Navigation data

`SiteGenerator.GenerateDataJs(...)` serializes:

- `rootNode` — wiki tree
- `tasks` — task metadata list
- `qa` — Q&A metadata list

## Sidebar behavior

`core.js` builds the sidebar in the browser:

- wiki tab renders the tree recursively
- tasks tab renders a flat list
- Q&A tab renders a flat list

## Path handling

Relative paths are calculated so pages can link correctly regardless of nesting depth.

## Mobile behavior

The sidebar is hidden off-screen on small screens and opened with a CSS transform class.

## Current-page handling

The script highlights the current page and expands parent folders in the tree.
