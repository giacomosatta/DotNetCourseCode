using Dapper;
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
    public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get";
        string stringParameters = string.Empty;

        DynamicParameters dynamicParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameters += $", @UserId = @UserIdParam";
            dynamicParameters.Add("@UserIdParam", userId, System.Data.DbType.Int32);
        }

        if (postId != 0)
        {
            stringParameters += $", @PostId = @PostIdParam";
            dynamicParameters.Add("@PostIdParam", postId, System.Data.DbType.Int32);
        }

        if (!string.Equals(searchParam, "None"))
        {
            stringParameters += $", @SearchValue = @SearchValueParam";
            dynamicParameters.Add("@SearchValueParam", searchParam, System.Data.DbType.String);
        }

        if (stringParameters.Length > 0)
            sql += stringParameters.Substring(1);

        return _dapper.LoadDataWithParameters<Post>(sql, dynamicParameters);
    }

    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = $"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParam";

        DynamicParameters dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value,System.Data.DbType.Int32);

        return _dapper.LoadDataWithParameters<Post>(sql, dynamicParameters);
    }

    [HttpPut("Post")]
    public IActionResult UpdatePost(Post postToAdd)
    {
        string sql = @$"EXEC TutorialAppSchema.spPosts_Upsert
                    @UserId = @UserIdParam" +
                    $", @PostTitle = @PostTitleParam" +
                    $", @PostContent = @PostContentParam";

        DynamicParameters dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value,System.Data.DbType.Int32);
        dynamicParameters.Add("@PostTitleParam", postToAdd.PostTitle,System.Data.DbType.String);
        dynamicParameters.Add("@PostContentParam", postToAdd.PostContent,System.Data.DbType.String);


        if (postToAdd.PostId > 0)
        {
            sql += $", @PostId = @PostIdParam";
            dynamicParameters.Add("@PostIdParam", postToAdd.PostId,System.Data.DbType.Int32);
        }

        if (!_dapper.ExecuteSqlWithParameters(sql, dynamicParameters)) throw new Exception("Failed to create new post!");

        return Ok();
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = @$"TutorialAppSchema.spPost_Delete @PostId = PostIdParam, @UserId = @UserIdParam";

        DynamicParameters dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@PostIdParam", postId,System.Data.DbType.Int32);
        dynamicParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value,System.Data.DbType.String);

        if (!_dapper.ExecuteSqlWithParameters(sql, dynamicParameters)) throw new Exception("Failed to delete post!");

        return Ok();
    }

}