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

### Image silder

<details>
<summary>Image slider html</summary>
<sm>

```html
<div class="slider">
<div class="slide">
	<img src="02-sync-request-asp.net-core.png" title="">
	<a class="source" href="https://code-maze.com/asynchronous-programming-with-async-and-await-in-asp-net-core/">Image source</a>
</div>
<div class="slide">
	<img src="03-async-request-asp.net-core.png" alt="" title="">
	<a class="source" href="https://code-maze.com/asynchronous-programming-with-async-and-await-in-asp-net-core/">Image source</a>
</div>
<p><button class="button-slider button-slider--prev"> &lt; </button>
<button class="button-slider button-slider--next"> &gt; </button></p>
</div>
```
</sm>
</details>

<details>
<summary>Image slider js</summary>
<sm>

```html
<script>
const setSlides = (slides, currentSlide) => {
  slides.forEach((slide, indx) => {
    slide.style.transform = `translateX(${(indx - currentSlide) * 100}%)`;
  });
}

const setSlider = () => {
  document.querySelectorAll(".slider").forEach((slider) => {
    const slides = slider.querySelectorAll(".slide");
    const nextSlide = slider.querySelector(".button-slider--next");
    const prevSlide = slider.querySelector(".button-slider--prev");
    const maxSlideIndex = slides.length - 1;
    let currentSlideIndex = 0;
    if (nextSlide) {
      nextSlide.onclick = () => {
        currentSlideIndex = (currentSlideIndex === maxSlideIndex) ? 0 : currentSlideIndex + 1;
        setSlides(slides, currentSlideIndex);
      };
    }
    if (prevSlide) {
      prevSlide.onclick = () => {
        currentSlideIndex = (currentSlideIndex === 0) ? maxSlideIndex : currentSlideIndex - 1;
        setSlides(slides, currentSlideIndex);
      };
    }
    setSlides(slides, 0);
  });
}

setSlider();
</script>
```
</sm>
</details>

</tabs>