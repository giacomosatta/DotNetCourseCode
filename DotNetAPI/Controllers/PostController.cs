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

    [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
    public IEnumerable<Post> GetPosts(int postId, int userId, string searchParam = "None")
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get";
        string parameters = string.Empty;

        if (userId != 0)
            parameters += $", @UserId = {userId}";

        if (postId != 0)
            parameters += $", @PostId = {userId}";

        if (!string.Equals(searchParam, "None"))
            parameters += $", @SearchValue = {searchParam}";

        if (parameters.Length > 0)
            sql += parameters.Substring(1);

        return _dapper.LoadData<Post>(sql);
    }

    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = $"EXEC TutorialAppSchema.spPosts_Get @UserId = '" + User.FindFirst("userId")?.Value + "'";

        return _dapper.LoadData<Post>(sql);
    }

    [HttpPut("Post")]
    public IActionResult UpdatePost(Post postToAdd)
    {
        string sql = @$"
                TutorialAppSchema.spPosts_Upsert
                    @UserId = " + User.FindFirst("userId")?.Value +
                    $", @PostTitle = '{postToAdd.PostTitle}'" +
                    $", @PostContent = '{postToAdd.PostContent}'";

        if (postToAdd.PostId > 0)
            sql += $", @PostId = {postToAdd.PostId}";

        if (!_dapper.ExecuteSql(sql)) throw new Exception("Failed to create new post!");

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

}