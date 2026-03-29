# Demo content format

This page shows the expected content layout for users of the generator.

## Folder structure

```text
MyWiki/
├─ _wiki/
│  ├─ intro.md
│  └─ docs/
│     └─ getting-started.md
├─ _tasks/
│  └─ improve-sidebar.md
└─ _qa/
   └─ how-does-build-work.md
```

## Minimal wiki page

```markdown
# Introduction

Some project notes here.
```

## Minimal task page

```markdown
---
name: Improve sidebar
status: Open
---

# Improve sidebar

Notes here.
```

## Minimal Q&A page

```markdown
---
question: How does the build work?
popularity: 1
---

# How does the build work?

Answer here.
```
