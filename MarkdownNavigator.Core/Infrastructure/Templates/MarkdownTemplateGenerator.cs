namespace MarkdownNavigator.Core.Infrastructure;

public static class MarkdownTemplateGenerator
{
    public static string GetWikiFolderTemplate(string folderName)
    {
        return $"# {folderName}\n\nThis file was auto-generated.";
    }

    public static string GetIndexTemplate()
    {
        return @"# Navigator.md

Local wiki for docs, tasks, and Q&A.

## Quick Syntax

| Item          | Example                                   |
|---------------|-------------------------------------------|
| Bold          | `**Text**`                                |
| Italic        | `*Text*`                                  |
| Bold + Italic | `***Text***`                              |
| Strike        | `~~Text~~`                                |
| Code          | `` `code` ``                              |
| Link          | `[Site](https://example.com)`             |
| Image         | `![Alt](image.jpg)`                       |
| Code emoji    | `🔥` · `🐞` · `🚀` · `🛠️` · `🔍` · `🤖`  |
| Notes emoji   | `✅` · `❌` · `⚠️` · `📌` · `💡` · `💀`  |

## Style snippet 

```markdown
<style>
r { color: Red }
sm { font-size: 0.6rem }
</style>
```

## Task list snippet

```markdown
- [x] Write the press release
- [ ] Update the website
- [ ] Contact the media
```

## Details snippet

````markdown
<details>
<summary>SqlScript</summary>

```sql
select * from table
```
</details>
````

## Tabs snippet

```markdown
!<tabs>
<tab> TabHeader1
</tabs>
```

## Image silder snippet

```html
<div class=""slider"">
<div class=""slide"">
	<img src=""02-sync-request-asp.net-core.png"" title="""">
	<span>Image source</span>
</div>
<div class=""slide"">
	<img src=""03-async-request-asp.net-core.png"" alt="""" title="""">
	<span><a href=""https://code-maze.com/asynchronous-programming-with-async-and-await-in-asp-net-core/"">Image source</a></span>
</div>
<p><button class=""button-slider button-slider--prev""> &lt; </button>
<button class=""button-slider button-slider--next""> &gt; </button></p>
</div>
```

## Task page

```markdown
---
name: TaskName
description: ShortDescription
status: `Open` · `Frozen` · `Closed`
link: https://example.com
---

# Task

## Links

- [Reference](https://example.com)

## Description

Describe the task here.

## Notes

- Key point 1
- Key point 2

```


## Q&A page

```markdown
---
question: How to debug async requests in ASP.NET Core?
context: Research notes for API performance
searchTags: [csharp, aspnetcore, async]
popularity: 0
---

# How to debug async requests in ASP.NET Core?

## Answer

Write the answer here.

## Notes

- Add examples
- Add pitfalls
- Add links to related pages

```";
    }
}
