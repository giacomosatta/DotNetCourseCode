using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;
    IMapper _mapper;
    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _entityFramework.Users.ToList();
    }


    [HttpGet("GetUsers/{userId}")]
    public User? GetSingleUser(int userId)
    {
        var user = _entityFramework.Users
                            .Where(u => u.UserId == userId)
                            .FirstOrDefault();

        if (user == null) throw new Exception("Failed to Get User");

        return user;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _entityFramework.Users
                           .Where(u => u.UserId == user.UserId)
                           .FirstOrDefault();

        if (userDb == null) throw new Exception("Failed to Get User");

        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Email = user.Email;
        userDb.Gender = user.Gender;

        return _entityFramework.SaveChanges() > 0 ? Ok() : throw new Exception("Failed to update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {

        User userDb = _mapper.Map<User>(user);
    
        _entityFramework.Add(userDb);
        return _entityFramework.SaveChanges() > 0 ? Ok() : throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _entityFramework.Users
                                 .Where(u => u.UserId == userId)
                                 .FirstOrDefault();

        if (userDb == null) throw new Exception("Failed to Get User");

        _entityFramework.Users.Remove(userDb);
        return _entityFramework.SaveChanges() > 0 ? Ok() : throw new Exception("Failed to delete User");
    }
}