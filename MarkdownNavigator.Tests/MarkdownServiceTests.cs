using MarkdownNavigator.Domain.Services;

namespace MarkdownNavigator.Tests
{
  public class MarkdownServiceTests
  {
    [Fact]
    public void GetSliderWithTwoImagesWithCustomHeight()
    {
      var markdownText = @"
@@slider 300px
![image1.jpg](Text for image 1)
![image2.jpg](Text for image 2)
@@
";

      var expectedHtml = @"
<div class=""slider"" style=""height: 300px;"">
<div class=""slide"">
<img src=""image1.jpg"">
<span>Text for image 1</span>
</div>
<div class=""slide"">
<img src=""image2.jpg"">
<span>Text for image 2</span>
</div>
<button class=""button-slider button-slider--prev""> &lt; </button>
<button class=""button-slider button-slider--next""> &gt; </button>
</div>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetSliderWithTwoImagesWithDefaultHeight()
    {
      var markdownText = @"
@@slider
![image1.jpg](Text for image 1)
![image2.jpg](Text for image 2)
@@
";

      var expectedHtml = @"
<div class=""slider"" style=""height: 350px;"">
<div class=""slide"">
<img src=""image1.jpg"">
<span>Text for image 1</span>
</div>
<div class=""slide"">
<img src=""image2.jpg"">
<span>Text for image 2</span>
</div>
<button class=""button-slider button-slider--prev""> &lt; </button>
<button class=""button-slider button-slider--next""> &gt; </button>
</div>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetSliderWithOneImageWithDefaultHeight()
    {
      var markdownText = @"
@@slider
![image.jpg](Text for image 1)
@@
";

      var expectedHtml = @"
<div class=""slider"" style=""height: 350px;"">
<div class=""slide"">
<img src=""image.jpg"">
<span>Text for image 1</span>
</div>
</div>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetSliderWithOneImageWithDefaultHeightWithoutText()
    {
      var markdownText = @"
@@slider
![image.jpg]()
@@
";

      var expectedHtml = @"
<div class=""slider"" style=""height: 350px;"">
<div class=""slide"">
<img src=""image.jpg"">
<span></span>
</div>
</div>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetDetailsWithSimpleText()
    {
      var markdownText = @"
@@details How we can do it?
It's very simple.
Some text.
@@
";

      var expectedHtml = @"
<details>
<summary>How we can do it?</summary>
<div><p>It's very simple.
Some text.</p>
</div>
</details>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetDetailsWithCodeBlock()
    {
      var markdownText = @"
@@details Sql query to get information about columns
The `INFORMATION_SCHEMA.COLUMNS` view allows you to get information about all columns for all tables and views within a database.

```sql
SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME='TableName'
```
@@
";

      var expectedHtml = @"
<details>
<summary>Sql query to get information about columns</summary>
<div><p>The <code>INFORMATION_SCHEMA.COLUMNS</code> view allows you to get information about all columns for all tables and views within a database.</p>
<pre><code class=""language-sql"">SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME='TableName'
</code></pre>
</div>
</details>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetNestedDetailsWithSimpleText()
    {
      var markdownText = @"
@@details How we can do it?
It's very simple.
Some text.
@@@details First Nested
Nested text 1.
@@@
Some text between.
@@@details Second Nested
Nested text 2.
@@@
@@
";

      var expectedHtml = @"
<details>
<summary>How we can do it?</summary>
<div><p>It's very simple.
Some text.</p>
<details>
<summary>First Nested</summary>
<div><p>Nested text 1.</p>
</div>
</details>
<p>Some text between.</p>
<details>
<summary>Second Nested</summary>
<div><p>Nested text 2.</p>
</div>
</details>
</div>
</details>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetHtmlWithTextAndSliderAndDetails()
    {
      var markdownText = @"# Header h1

## Header h2

[Github link](https://github.com/)

@@slider
![image1.jpg](Text for image 1)
![image2.jpg](Text for image 2)
@@

Some text in a paragraph.

@@details How we can do it?
It's very simple.
Some text.
@@

Some other text in the paragraph with **bold font**.
";

      var expectedHtml = @"
<h1 id=""header-h1"">Header h1</h1>
<h2 id=""header-h2"">Header h2</h2>
<p><a href=""https://github.com/"" target=""_blank"" rel=""noopener noreferrer"">Github link</a></p>
<div class=""slider"" style=""height: 350px;"">
<div class=""slide"">
<img src=""image1.jpg"">
<span>Text for image 1</span>
</div>
<div class=""slide"">
<img src=""image2.jpg"">
<span>Text for image 2</span>
</div>
<button class=""button-slider button-slider--prev""> &lt; </button>
<button class=""button-slider button-slider--next""> &gt; </button>
</div>
<p>Some text in a paragraph.</p>
<details>
<summary>How we can do it?</summary>
<div><p>It's very simple.
Some text.</p>
</div>
</details>
<p>Some other text in the paragraph with <strong>bold font</strong>.</p>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetTwoTabsPanelWithSimpleText()
    {
      var markdownText = @"
@@tabs tabs
@@@tab First tab
### First tab
Some Text.
@@@tab Second tab
### Second tab
Some Text.
@@@tab Third tab
### Third tab
Some Text.
@@
Some text between.
@@tabs tabs2
@@@tab First tab
### First tab
Some Text.
@@
";

      var expectedHtml = @"
<div class=""tabs"">
<input class=""input"" name=""tabs"" type=""radio"" id=""tabs-1"" checked=""checked""/>
<label class=""label"" for=""tabs-1"">First tab</label>
<div class=""panel"">
<h3 id=""first-tab"">First tab</h3>
<p>Some Text.</p>
</div>
<input class=""input"" name=""tabs"" type=""radio"" id=""tabs-2""/>
<label class=""label"" for=""tabs-2"">Second tab</label>
<div class=""panel"">
<h3 id=""second-tab"">Second tab</h3>
<p>Some Text.</p>
</div>
<input class=""input"" name=""tabs"" type=""radio"" id=""tabs-3""/>
<label class=""label"" for=""tabs-3"">Third tab</label>
<div class=""panel"">
<h3 id=""third-tab"">Third tab</h3>
<p>Some Text.</p>
</div>
</div>
<p>Some text between.</p>
<div class=""tabs"">
<input class=""input"" name=""tabs2"" type=""radio"" id=""tabs2-1"" checked=""checked""/>
<label class=""label"" for=""tabs2-1"">First tab</label>
<div class=""panel"">
<h3 id=""first-tab"">First tab</h3>
<p>Some Text.</p>
</div>
</div>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }
  }
}
