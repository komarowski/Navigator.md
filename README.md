# Navigator.md

**Navigator.md** is a simple console app for generating html docs from markdown files for use on your local machine. It also generates a tree view of these files for quick navigation. This project is complemented by a web application that provides a simple interface for editing the content.

The main purpose of this application is to organize your bookmarks, notes, articles, and code examples in a beautiful and efficient manner, storing them securely on your local machine.

## Features

- Converts .md files to .html with a pure HTML layout
- Responsive web design
- Shows a tree view of markdown files
- Shows table of contents (h1, h2 tags)
- Tracks changes in markdown files and modifies html accordingly
- Extended markdown syntax for tabs, links
- Copying code blocks to the clipboard
- Basic editing with live html preview

## Demo

![](https://github.com/komarowski/Navigator.md/blob/main/demo/demo.gif)

## Project structure

```bash
├── /MarkdownNavigator.Console         # Main console app for generating html docs
│
├── /MarkdownNavigator.Domain          # Shared business logic
│
├── /MarkdownNavigator.Infrastructure  # Managing resource files
│   ├── /Resources
│       ├── /assets                    # Shared static files
│
├── /MarkdownNavigator.React           # Frontend React app for markdown editing
│
├── /MarkdownNavigator.Tests           # Unit and intergration tests 
│
├── /MarkdownNavigator.Web             # Backend minimal API app for markdown editing
│
└── /StaticFilesManager                # Managing shared static files
```

## Conventions

### App settings

 - SourceFolder: path to the source folder containing your markdown files.
 - Server: URL of the local web app for editing markdown files.

Here is an example configuration:

```json
{
  "SourceFolder": "C:\\Documents\\Wiki",
  "Server": "https://localhost:7024"
}
```

### Managing static files

To update static files across multiple projects, you can either run `StaticFilesManager` or manually follow these steps:

 - Copy the contents of `MarkdownNavigator.Infrastructure/Resources/assets` to `MarkdownNavigator.React/public/assets` (except `main.js`)
 - After building the React app, copy the contents of `MarkdownNavigator.React/build` to `MarkdownNavigator.Web/wwwroot`