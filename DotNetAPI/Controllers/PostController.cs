using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts()
    {
        string sql = @"SELECT [PostId],
                              [UserId],
                              [PostTitle],
                              [PostContent],
                              [PostCreated],
                              [PostUpdated]
                    FROM TutorialAppSchema.Posts";

        return _dapper.LoadData<Post>(sql);
    }

    [HttpGet("PostSingle/{postId}")]
    public Post GetPostSingle(int postId)
    {
        string sql = @$"SELECT [PostId],
                              [UserId],
                              [PostTitle],
                              [PostContent],
                              [PostCreated],
                              [PostUpdated]
                    FROM TutorialAppSchema.Posts
                    WHERE PostId = {postId}";

        return _dapper.LoadDataSingle<Post>(sql);
    }

    [HttpGet("PostsByUser/{userId}")]
    public IEnumerable<Post> GetPostsByUser(int userId)
    {
        string sql = @$"SELECT [PostId],
                              [UserId],
                              [PostTitle],
                              [PostContent],
                              [PostCreated],
                              [PostUpdated]
                    FROM TutorialAppSchema.Posts
                    WHERE UserId = {userId}";

        return _dapper.LoadData<Post>(sql);
    }

    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = @$"SELECT [PostId],
                              [UserId],
                              [PostTitle],
                              [PostContent],
                              [PostCreated],
                              [PostUpdated]
                    FROM TutorialAppSchema.Posts
                    WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

        return _dapper.LoadData<Post>(sql);
    }

    [HttpPost("Post")]
    public IActionResult AddPost(PostToAddDto postToAdd)
    {
        string sql = @$"
         INSERT INTO TutorialAppSchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]) VALUES(" + User.FindFirst("userId")?.Value
                + ",'" + postToAdd.PostTitle
                + "','" + postToAdd.PostContent
                + "',GETDATE(), GETDATE() )";

        if (!_dapper.ExecuteSql(sql)) throw new Exception("Failed to create new post!");

        return Ok();
    }

    [HttpPut("Post")]
    public IActionResult EditPost(PostToEditDto postToEdit)
    {
        string sql = @$"
                UPDATE TutorialAppSchema.Posts 
                        SET PostContent = '{postToEdit.PostContent}', PostTitle ='{postToEdit.PostTitle}', PostUpdated = GETDATE()
                WHERE PostId = {postToEdit.PostId}
                AND UserId = " + User.FindFirst("userId")?.Value;

        if (!_dapper.ExecuteSql(sql)) throw new Exception("Failed to edit new post!");

        return Ok();
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = @$"DELETE FROM TutorialAppSchema.Posts WHERE PostId = {postId}
                        AND UserId = " + User.FindFirst("userId")?.Value;

        if (!_dapper.ExecuteSql(sql)) throw new Exception("Failed to delete post!");

        return Ok();
    }

     [HttpGet("PostsBySearch/{searchParam}")]
    public IEnumerable<Post> PostsBySearch(string searchParam)
    {
        string sql = @$"SELECT [PostId],
                              [UserId],
                              [PostTitle],
                              [PostContent],
                              [PostCreated],
                              [PostUpdated]
                    FROM TutorialAppSchema.Posts
                    WHERE PostTitle LIKE '%{searchParam}%' 
                    OR PostContent LIKE '%{searchParam}%'";


        return _dapper.LoadData<Post>(sql);
    }

}