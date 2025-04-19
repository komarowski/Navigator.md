# Navigator.md help

<style>
sm { font-size: 0.6rem }
</style>

!<tabs>
<tab> Basic

### Emphasis

- **Bold** `**`
- *Italic* `*`
- ***Bold and Italic*** `***`

### Links

 - Image `![ImageAltText](image.jpg)`
 - Link `[SiteName](SiteUrl)`

### Emoji

`🔥` - `⚠️` - `⭐` - `📌` -  `❗` - `❓`

<tab> Advanced

### Emphasis

- ~~Strikethrough~~ `~~`
- ==Highlight== `==`

### Style

```markdown
<style>
r { color: Red }
sm { font-size: 0.6rem }
</style>
```

### Table

```markdown
| Syntax      | Description |
| ----------- | ----------- |
| Header      | Title       |
| Paragraph   | Text        |
```
### Task Lists

```markdown
- [x] Write the press release
- [ ] Update the website
- [ ] Contact the media
```

<tab> Templates

### Details  with code block

````markdown
<details>
<summary>SqlScript</summary>

```sql
select * from table
```
</details>
````

### Tabs

- `!<tabs>`
- `<tab> TabHeader1`
- `</tabs>`

### Textarea with JS

````markdown
<textarea class="textarea-js">
console. log("Hello World");
</textarea>
````

### Image silder

```html
<div class="slider">
<div class="slide">
	<img src="02-sync-request-asp.net-core.png" title="">
	<span>Image source</span>
</div>
<div class="slide">
	<img src="03-async-request-asp.net-core.png" alt="" title="">
	<span><a href="https://code-maze.com/asynchronous-programming-with-async-and-await-in-asp-net-core/">Image source</a></span>
</div>
<p><button class="button-slider button-slider--prev"> &lt; </button>
<button class="button-slider button-slider--next"> &gt; </button></p>
</div>
```

</tabs>