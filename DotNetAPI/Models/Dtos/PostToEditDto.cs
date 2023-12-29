namespace DotnetAPI.Models.Dtos;

public partial class PostToEditDto
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string PostTitle { get; set; } ="";
    public string PostContent { get; set; }="";
    public DateTime PostCreated { get; set; }
   public DateTime PostUpdated { get; set; }    
}