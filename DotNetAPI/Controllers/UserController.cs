using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }


    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>(@"SELECT GETDATE()");
    }


    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"SELECT [UserId],
                              [FirstName],
                              [LastName],
                              [Email],
                              [Gender],
                              [Active] 
                     FROM TutorialAppSchema.Users";

        return _dapper.LoadData<User>(sql);

    }


    [HttpGet("GetUsers/{userId}")]
    public User GetSingleUser(string userId)
    {
        string sql = @$"SELECT [UserId],
                              [FirstName],
                              [LastName],
                              [Email],
                              [Gender],
                              [Active] 
                     FROM TutorialAppSchema.Users
                     WHERE UserId = {userId}";

        return _dapper.LoadDataSingle<User>(sql);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = $@"
            UPDATE TutorialAppSchema.Users
                SET [FirstName] ='{user.FirstName}',
                    [LastName]='{user.LastName}',
                    [Email]='{user.Email}',
                    [Gender]='{user.Gender}',
                    [Active] = '{user.Active}'
                WHERE UserId = {user.UserId}";

        Console.WriteLine(sql);

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @$"INSERT INTO TutorialAppSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active] 
                        )VAlUES(
                            '{user.FirstName}',
                            '{user.LastName}',
                            '{user.Email}',
                            '{user.Gender}',
                            '{user.Active}'
                        )";
        Console.WriteLine(sql);

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to create User");
    }
}