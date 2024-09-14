namespace MarkdownNavigator.Web.DTO
{
  public class ResultTextDTO : ResultDTO
  {
    public string? Text { get; set; }

    public ResultTextDTO() 
    {
    }

    public ResultTextDTO(string text, bool isError = false)
    {
      IsError = isError;
      if (isError)
      {
        ErrorMessage = text;
      }
      else
      {
        Text = text;
      }   
    }
  }
}
