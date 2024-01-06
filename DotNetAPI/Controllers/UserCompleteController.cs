using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
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
        string parameters = string.Empty;

        if (userId != 0)
            parameters += $", @UserId={userId}";

        if (isActive)
            parameters += $", @Active={isActive}";

        if (parameters.Length > 0)
            sql += parameters.Substring(1); //, parameters.Length);

        return _dapper.LoadData<UserComplete>(sql);
    }

    [HttpPut("UpsertUser")]
    public IActionResult EditUser(UserComplete user)
    {
        string sql = $@"
            EXEC TutorialAppSchema.spUser_Upsert
                SET @FirstName ='{user.FirstName}',
                    @LastName ='{user.LastName}',
                    @Email ='{user.Email}',
                    @Gender ='{user.Gender}',
                    @Active = '{user.Active}',
                    @JobTitle = '{user.JobTitle}',
                    @Department = '{user.Department}',
                    @Salary = '{user.Salary}',
                    @UserId = {user.UserId}";

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @$"EXEC TutorialAppSchema.spUser_Delete @UserId = {userId}";

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to delete User");
    }
}