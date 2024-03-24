using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.App.Controllers;
using Server.App.Db.Contexts;

namespace Tests;
// Hello f*ckig world
public class RegistrationServiceTest
{
    ApplicationDbContext _db;
    private RegistrationService _registrationService;

    public RegistrationServiceTest()
    {
        // Create an in-memory database with the fake data
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _db = new ApplicationDbContext(options);
        _registrationService = new RegistrationService (_db, new HashService());

        _db.Users.Add(new User
        {
            Name = "vlad",
            Id = 1,
            LoginHash="as",
            PasswordHash=""
        });
        _db.Users.Add(new User
        {
            Name = "dima",
            Id = 2,
            LoginHash="sd",
            PasswordHash=""
        });
        _db.Users.Add(new User
        {
            Name = "sasha",
            Id = 3,
            LoginHash="a",
            PasswordHash=""
        });
        _db.Groups.Add(new Group{
            Id=1,
            Name="group 1",
            PasswordHash="123"
        });
        _db.Groups.Add(new Group{
            Id=2,
            Name="group 2",
            PasswordHash="123"
        });
        _db.UserGroups.Add(new UserGroup{
            GroupId=1,
            UserId=1,
            IsOwner=true
        });
        _db.UserGroups.Add(new UserGroup{
            GroupId=1,
            UserId=2,
            IsOwner=false
        });
        _db.UserGroups.Add(new UserGroup{
            GroupId=2,
            UserId=1,
            IsOwner=false
        });
        _db.SaveChanges();

    }
    [Fact]
    public void Register()
    {
        var res = _registrationService.Register("vasya", "123", "vasya").Result;
        Assert.True(_db.Users.Contains(res));
    }
    [Fact]
    public void LoginExistRegister_Failed()
    {
        var res = _registrationService.Register("vasya", "", "");
    }
}
