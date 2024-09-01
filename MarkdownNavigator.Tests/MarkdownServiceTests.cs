using MarkdownNavigator.Domain.Services;

namespace MarkdownNavigator.Tests
{
  public class MarkdownServiceTests
  {
    [Fact]
    public void GetSliderWithTwoImagesWithCustomHeight()
    {
      var markdownText = @"
!<slider> 300px
![image1.jpg](Text for image 1)
![image2.jpg](Text for image 2)
</slider>
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
!<slider>
![image1.jpg](Text for image 1)
![image2.jpg](Text for image 2)
</slider>
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
!<slider>
![image.jpg](Text for image 1)
</slider>
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
!<slider>
![image.jpg]()
</slider>
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
    public void GetHtmlWithTextAndSliderAndLink()
    {
      var markdownText = @"# Header h1

## Header h2

[Github link](https://github.com/)

!<slider>
![image1.jpg](Text for image 1)
![image2.jpg](Text for image 2)
</slider>

Some text in a paragraph.

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
<p>Some other text in the paragraph with <strong>bold font</strong>.</p>
";

      var actualHtml = MarkdownService.ConvertToHtml(markdownText);

      Assert.Equal(expectedHtml.NormalizeLineEndings().Trim(), actualHtml.NormalizeLineEndings().Trim());
    }

    [Fact]
    public void GetTwoTabsPanelWithSimpleText()
    {
      var markdownText = @"
!<tabs>
<tab> First tab
### First tab
Some Text.
<tab> Second tab
### Second tab
Some Text.
<tab> Third tab
### Third tab
Some Text.
</tabs>
Some text between.
!<tabs> secondTabsName
<tab> First tab
### First tab
Some Text.
</tabs>
";

      var expectedHtml = @"
<div class=""tabs"">
<input class=""input"" name=""default"" type=""radio"" id=""default-1"" checked=""checked""/>
<label class=""label"" for=""default-1"">First tab</label>
<div class=""panel"">
<h3 id=""first-tab"">First tab</h3>
<p>Some Text.</p>
</div>
<input class=""input"" name=""default"" type=""radio"" id=""default-2""/>
<label class=""label"" for=""default-2"">Second tab</label>
<div class=""panel"">
<h3 id=""second-tab"">Second tab</h3>
<p>Some Text.</p>
</div>
<input class=""input"" name=""default"" type=""radio"" id=""default-3""/>
<label class=""label"" for=""default-3"">Third tab</label>
<div class=""panel"">
<h3 id=""third-tab"">Third tab</h3>
<p>Some Text.</p>
</div>
</div>
<p>Some text between.</p>
<div class=""tabs"">
<input class=""input"" name=""secondTabsName"" type=""radio"" id=""secondTabsName-1"" checked=""checked""/>
<label class=""label"" for=""secondTabsName-1"">First tab</label>
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
