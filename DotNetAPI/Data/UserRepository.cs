using DotnetAPI.Data.Interfaces;
using DotnetAPI.Models;

namespace DotnetAPI.Data;

class UserRepository : IUserRepository
{

    DataContextEF _entityFramework;

    public UserRepository(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }

    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entityToAdd)
    {
        if (entityToAdd == null) return;

        _entityFramework.Add(entityToAdd);
    }

    public void RemoveEntity<T>(T entityToRemove)
    {
        if (entityToRemove == null) return;

        _entityFramework.Remove(entityToRemove);
    }

    public IEnumerable<User> GetUsers()
    {
        return _entityFramework.Users.ToList();
    }

    public User? GetSingleUser(int userId)
    {
        return _entityFramework.Users
                            .Where(u => u.UserId == userId)
                            .FirstOrDefault();

    }

    public UserSalary? GetSingleUserSalary(int userId)
    {
        return _entityFramework.UserSalary
                            .Where(u => u.UserId == userId)
                            .FirstOrDefault();
    }

    public UserJobInfo? GetSingleUserJobInfo(int userId)
    {
        return _entityFramework.UserJobInfo
                            .Where(u => u.UserId == userId)
                            .FirstOrDefault();
    }

}