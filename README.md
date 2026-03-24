# Navigator.md

**Navigator.md** is a simple console app for generating HTML docs from markdown files for use on your local machine. It generates a navigable wiki with tree view, task tracking, and Q&A sections.

The main purpose of this application is to organize your bookmarks, notes, articles, and code examples in a beautiful and efficient manner, storing them securely on your local machine.

## Features

- Converts `.md` files into `.html`
- Generates a static local website with no server required
- Responsive layout for desktop and mobile
- Three content sections: Wiki, Tasks, and Q&A
- Table of contents based on headings
- Watches source files and updates generated output
- Supports extended Markdown features such as tabs and custom links
- Adds copy-to-clipboard support for code blocks

## Demo

![](https://github.com/komarowski/Navigator.md/blob/main/demo/demo.gif)

## How it works

```bash
SourceFolder/
├── index.html              # Generated home page
├── data.js                 # Generated navigation/content data
├── assets/                 # Copied static resources
├── _wiki/                  # Wiki articles
├── _tasks/                 # Task items
└── _qa/                    # Q&A articles
```

1. The app reads content from the configured SourceFolder.
2. It scans the `_wiki`, `_tasks`, and `_qa` folders.
3. Markdown files are parsed and converted into site content models.
4. HTML pages, navigation data, and static assets are generated into the output folder.
5. During runtime, the app watches the source folder and updates the site when files change.

## Project structure

```bash
├── /MarkdownNavigator.Console      # Main console app
├── /MarkdownNavigator.Core         # Core business logic and infrastructure
│   ├── /Application                # Use cases and orchestration
│   ├── /Domain                     # Entities, services, markdown processing
│   ├── /Infrastructure             # HTML generation, file output, embedded resources
│   └── /Resources
│       └── /assets                 # Static resources (CSS, JS, fonts, plugins)
└── /MarkdownNavigator.Tests        # Unit and integration tests 
```

