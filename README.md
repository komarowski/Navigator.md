# Navigator.md

**Navigator.md** is a simple console app for generating html docs from markdown files for use on your local machine. It also generates a tree view of these files for quick navigation.

The main purpose of this application is to organize your bookmarks, notes, articles, and code examples in a beautiful and efficient manner, storing them securely on your local machine.

## Features

- Converts .md files to .html with a pure HTML layout
- Responsive web design
- Shows a tree view of markdown files
- Shows table of contents (h1, h2 tags)
- Tracks changes in markdown files and modifies html accordingly
- Extended markdown syntax for tabs, links
- Copying code blocks to the clipboard

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
│       ├── /assets                    # Static resources
│       │   ├──/core                   # Core styles and JavaScript
│       │   ├──/fonts                  # Roboto font files
│       │   ├──/plugins                # Optional plugins to extend functionality
│       ├── /templates                 # HTML template for generating docs
│
└── /MarkdownNavigator.Tests           # Unit and intergration tests 
```

## Conventions

### App settings

 - SourceFolder: path to the source folder containing your markdown files.
 - PluginList: list of enabled plugins (injects CSS and JS links into the template)

Here is an example configuration:

```json
{
  "SourceFolder": "C:\\Documents\\Wiki",
  "PluginList": [ "code", "prism", "slider" ]
}
```