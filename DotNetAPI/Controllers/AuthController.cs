using System.Security.Cryptography;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
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
    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _config = config;
        _authHelper = new AuthHelper(config);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password != userForRegistration.PasswordConfirm) throw new Exception("Passwords do not match");

        string sqlCheckUserExists = $"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '{userForRegistration.Email}'";

        IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExists);

        if (existingUser.Count() > 0) throw new Exception("User with this email already exists!");

        byte[] passwordSalt = new byte[128 / 8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

        string sqlAddAuth = @$"INSERT INTO TutorialAppSchema.Auth ([Email],
                                    [PasswordHash],
                                    [PasswordSalt]) VALUES ('{userForRegistration.Email}' 
                                    ,@PasswordHash , @PasswordSalt)";

        List<SqlParameter> sqlParameters = new List<SqlParameter>();
        SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", System.Data.SqlDbType.VarBinary)
        {
            Value = passwordSalt
        };
        SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", System.Data.SqlDbType.VarBinary)
        {
            Value = passwordHash
        };

        sqlParameters.Add(passwordHashParameter);
        sqlParameters.Add(passwordSaltParameter);

        if (!_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters)) throw new Exception("Failed to register user.");

        string sqlAddUser = @$"INSERT INTO TutorialAppSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active] 
                        )VAlUES(
                            '{userForRegistration.FirstName}',
                            '{userForRegistration.LastName}',
                            '{userForRegistration.Email}',
                            '{userForRegistration.Gender}',
                            '1'
                        )";

        if (!_dapper.ExecuteSql(sqlAddUser)) throw new Exception("Failed to Add User");

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @$"SELECT [PasswordHash],
                                            [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '{userForLogin.Email}'";

        UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

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