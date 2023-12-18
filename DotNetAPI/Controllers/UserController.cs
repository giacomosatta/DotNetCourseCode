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

    [HttpGet("GetUserJobInfos")]
    public IEnumerable<UserSalary> GetUserJobInfos()
    {
        string sql = @"SELECT [UserId],
                              [JobTitle],
                              [Department]
                     FROM TutorialAppSchema.UserJobInfo";

        return _dapper.LoadData<UserSalary>(sql);

    }

     [HttpGet("GetUserSalaries")]
    public IEnumerable<UserSalary> GetUserSalaries()
    {
        string sql = @"SELECT [UserId],
                              [Salary],
                              [AvgSalary]
                     FROM TutorialAppSchema.UserSalary";

        return _dapper.LoadData<UserSalary>(sql);
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

    [HttpGet("GetSingleUserJobInfos/{userId}")]
    public UserSalary GetSingleUserJobInfo(string userId)
    {
        string sql = @$"SELECT [UserId],
                              [JobTitle],
                              [Department]
                     FROM TutorialAppSchema.UserJobInfo
                     WHERE UserId = {userId}";

        return _dapper.LoadDataSingle<UserSalary>(sql);

    }


    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(string userId)
    {
        string sql = @$"SELECT [UserId],
                              [Salary],
                              [AvgSalary]
                     FROM TutorialAppSchema.UserSalary
                     WHERE UserId = {userId}";

        return _dapper.LoadDataSingle<UserSalary>(sql);

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

    
    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = $@"
            UPDATE TutorialAppSchema.UserJobInfo
                SET [JobTitle] ='{userJobInfo.JobTitle}',
                    [Department]='{userJobInfo.Department}'
                WHERE UserId = {userJobInfo.UserId}";

        Console.WriteLine(sql);

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to update UserJobInfo");
    }

     [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = $@"
            UPDATE TutorialAppSchema.UserSalary
                SET [Salary] ='{userSalary.Salary}',
                    [AvgSalary]='{userSalary.AvgSalary}'
                WHERE UserId = {userSalary.UserId}";

        Console.WriteLine(sql);

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to update UserSalary");
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


    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @$"INSERT INTO TutorialAppSchema.UserJobInfo(
                            [UserId],
                            [JobTitle],
                            [Department]
                        )VAlUES(
                            '{userJobInfo.UserId}',
                            '{userJobInfo.JobTitle}',
                            '{userJobInfo.Department}'
                        )";
        Console.WriteLine(sql);

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to create UserJobInfo");
    }


    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        string sql = @$"INSERT INTO TutorialAppSchema.UserSalary(
                            [UserId],
                            [Salary],
                            [AvgSalary]
                        )VAlUES(
                            '{userSalary.UserId}',
                            '{userSalary.Salary}',
                            '{userSalary.AvgSalary}'
                        )";
        Console.WriteLine(sql);

        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to create UserSalary");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @$"DELETE FROM TutorialAppSchema.Users
                        WHERE UserId = {userId}";
        Console.WriteLine(sql);
        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to delete User");
    }

     [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @$"DELETE FROM TutorialAppSchema.UserJobInfo
                        WHERE UserId = {userId}";
        Console.WriteLine(sql);
        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to delete UserJobInfo");
    }

      [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = @$"DELETE FROM TutorialAppSchema.UserSalary
                        WHERE UserId = {userId}";
        Console.WriteLine(sql);
        return _dapper.ExecuteSql(sql) ? Ok() : throw new Exception("Failed to delete UserSalary");
    }
}