using System.Security.Cryptography;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private IConfiguration _config;
    private readonly AuthHelper _authHelper;
    private readonly IMapper _mapper;

    private readonly ReusableSql _reusableSql;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _config = config;
        _authHelper = new AuthHelper(config);
        _reusableSql = new ReusableSql(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserForRegistrationDto, UserComplete>();
        }));


    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password != userForRegistration.PasswordConfirm) throw new Exception("Passwords do not match");

        string sqlCheckUserExists = $"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '{userForRegistration.Email}'";

        IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExists);

        if (existingUser.Count() > 0) throw new Exception("User with this email already exists!");

        UserForLoginDto userForResetPassword = new UserForLoginDto()
        {
            Email = userForRegistration.Email,
            Password = userForRegistration.Password
        };

        if (!_authHelper.SetPassword(userForResetPassword)) throw new Exception("Failed to register userForRegistration.");

        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
        userComplete.Active = true;

        return _reusableSql.UpsertUser(userComplete) ? Ok() : throw new Exception("Failed to update User");
    }

    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
        if (!_authHelper.SetPassword(userForSetPassword)) throw new Exception("Failed to update password");

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @$"EXEC TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

        // List<SqlParameter> sqlParameters = new List<SqlParameter>();

        //    SqlParameter emailParameter = new SqlParameter("@EmailParam", System.Data.SqlDbType.VarChar)
        // {
        //     Value = userForLogin.Email
        // };

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@EmailParam", userForLogin.Email, System.Data.DbType.String);

        UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

        byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

        // if(passwordHash == userForLoginConfirmation.PasswordHash) Won't work

        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
                return StatusCode(401, "Incorrect password");
        }

        string userIdSql = $"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '{userForLogin.Email}'";

        int? userId = _dapper.LoadDataSingle<int>(userIdSql);
        if (userId == null) throw new Exception("User not found");

        return Ok(new Dictionary<string, string>{
            {"token",_authHelper.CreateToken(userId.Value)}
        });
    }

    [HttpGet("RefreshToken")]
    public string RefreshToken()
    {
        string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
           User.FindFirst("userId")?.Value + "'";

        int userId = _dapper.LoadDataSingle<int>(userIdSql);

        return _authHelper.CreateToken(userId);
    }
}