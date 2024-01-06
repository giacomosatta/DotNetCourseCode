using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSql(config);
    }


    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>(@"SELECT GETDATE()");
    }


    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @$"EXEC TutorialAppSchema.spUsers_Get";
        string stringParameters = string.Empty;

        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameters += $", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, System.Data.DbType.Int32);
        }

        if (isActive)
        {
            stringParameters += $", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, System.Data.DbType.Boolean);
        }

        if (stringParameters.Length > 0)
            sql += stringParameters.Substring(1); //, parameters.Length);

        return _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        return _reusableSql.UpsertUser(user) ? Ok() : throw new Exception("Failed to update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @$"EXEC TutorialAppSchema.spUser_Delete @UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@UserIdParameter", userId, System.Data.DbType.Int32);

        return _dapper.ExecuteSqlWithParameters(sql, sqlParameters) ? Ok() : throw new Exception("Failed to delete User");
    }
}