using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
using Server.App.Controllers;

namespace Tests;
// Hello f*ckig world
public class LoginTests
{
    ApplicationDbContext _db;
    private LoginService _userGroupManager;

    public LoginTests()
    {
        // Create an in-memory database with the fake data
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _db = new ApplicationDbContext(options);
        _userGroupManager = new LoginService(_db, new HashService());

        _db.Users.Add(new Server.User
        {
            Name = "vlad",
            Id = 1,
            LoginHash="",
            PasswordHash=""
        });
        _db.Users.Add(new Server.User
        {
            Name = "dima",
            Id = 2,
            LoginHash="",
            PasswordHash=""
        });
        _db.Users.Add(new Server.User
        {
            Name = "sasha",
            Id = 3,
            LoginHash="",
            PasswordHash=""
        });

        _db.Groups.Add(new Server.Group{
            Id=1,
            Name="group 1",
            PasswordHash="123"
        });

        _db.Groups.Add(new Server.Group{
            Id=2,
            Name="group 2",
            PasswordHash="123"
        });
        _db.Groups.Add(new Server.Group
        {
            Id = 3,
            Name = "group 3",
            PasswordHash = "241"
        });

        _db.UserGroups.Add(new Server.UserGroup{
            GroupId=1,
            UserId=1,
            IsOwner=true
        });

        _db.UserGroups.Add(new Server.UserGroup{
            GroupId=1,
            UserId=2,
            IsOwner=false
        });

        _db.UserGroups.Add(new Server.UserGroup{
            GroupId=2,
            UserId=1,
            IsOwner=false
        });
        _db.SaveChanges();
    }
    [Fact]
    public void Login()
    {
        var res = _userGroupManager.Login()
    }
}