# Navigator.md

**Navigator.md** is a simple app for generating html docs from markdown files for use on your local machine. It also generates a tree view of these files for quick navigation and supports simple editing. 

The main purpose of this application is to organize your bookmarks, notes, articles, and code examples in a beautiful and efficient manner, storing them securely on your local machine.

## Features

- Converts .md files to .html with a pure HTML layout
- Responsive web design
- Shows a tree view of markdown files
- Shows table of contents (h1, h2 tags)
- Tracks changes in markdown files and modifies html accordingly
- Extended markdown syntax for details, image slider
- Editing a markdown file

## Demo

![](https://github.com/komarowski/Navigator.md/blob/main/demo/demo.gif)

## Conventions

### App settings section

App settings locate in appsettings.json in `CustomSettings` section. Multiple settings configurations can be saved.

```json
{
	"CustomSettings": {
	  "SettingsList": [
		{
		  "Name": "Settings name 1",
		  "SourceFolder": "Path\\To\\Folder\\With\\Markdown\\Files",
		  "IsExport": false,
		  "IsStandalone": false,
		  "IsWebServer": true
		},
		{
		  "Name": "Settings name 2",
		  "SourceFolder": "Path\\To\\Folder\\With\\Markdown\\Files2",
		  "IsExport": true,
		  "IsStandalone": false,
		  "IsWebServer": false
		}
	  ]
	}
}
```
