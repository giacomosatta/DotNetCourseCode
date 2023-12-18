using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Data.Interfaces;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    IMapper _mapper;

    IUserRepository _userRepository;

    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<UserSalary, UserSalary>();
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _userRepository.GetUsers();
    }

    [HttpGet("GetUsers/{userId}")]
    public User? GetSingleUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetSingleUser(user.UserId);

        if (userDb == null) throw new Exception("Failed to Get User");

        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Email = user.Email;
        userDb.Gender = user.Gender;

        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {

        User userDb = _mapper.Map<User>(user);

        _userRepository.AddEntity(userDb);
        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _userRepository.GetSingleUser(userId);

        if (userDb == null) throw new Exception("Failed to Get User");

        _userRepository.RemoveEntity(userDb);
        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to delete User");
    }


    [HttpGet("GetUserJobInfo/{userId}")]
    public UserJobInfo? GetUserJobInfo(int userId)
    {
        return _userRepository.GetSingleUserJobInfo(userId);
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        UserJobInfo? userJobInfoDb = _userRepository.GetSingleUserJobInfo(userJobInfo.UserId);

        if (userJobInfoDb == null) throw new Exception("Failed to Get UserJobInfo");

        _mapper.Map(userJobInfo, userJobInfoDb);


        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to update UserJobInfo");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {


        _userRepository.AddEntity(userJobInfo);
        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to Add User Job Info");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        UserJobInfo? userJobInfoDb = _userRepository.GetSingleUserJobInfo(userId);

        if (userJobInfoDb == null) throw new Exception("Failed to Get User Job Info");

        _userRepository.RemoveEntity(userJobInfoDb);
        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to delete UserJobInfo");
    }



    [HttpGet("GetUserSalary/{userId}")]
    public UserSalary? GetUserSalary(int userId)
    {
        return _userRepository.GetSingleUserSalary(userId);
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        UserSalary? userSalaryDb = _userRepository.GetSingleUserSalary(userSalary.UserId);

        if (userSalaryDb == null) throw new Exception("Failed to Get UserSalary");

        _mapper.Map(userSalary, userSalaryDb);
        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to update UserSalary");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {


        _userRepository.AddEntity(userSalary);
        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to Add User Salary");
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userSalaryDb = _userRepository.GetSingleUserSalary(userId);

        if (userSalaryDb == null) throw new Exception("Failed to Get User Salary");

        _userRepository.RemoveEntity(userSalaryDb);
        return _userRepository.SaveChanges() ? Ok() : throw new Exception("Failed to delete User Salary");
    }
}