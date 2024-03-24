using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;

namespace Tests;
// Hello f*ckig world
public class UserGroupManagerTests
{
    ApplicationDbContext _db;
    private UserGroupManager _userGroupManager;

    public UserGroupManagerTests()
    {
        // Create an in-memory database with the fake data
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _db = new ApplicationDbContext(options);
        _userGroupManager = new UserGroupManager(_db);

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
    public void GetUserGroups()
    {
        var res = _userGroupManager.GetUserGroups(1).Result;
        var g1 = new Server.Group()
        {
            Id = 1,
            Name = "group 1",
            PasswordHash = "123"
        };
        var g2 = new Server.Group()
        {
            Id = 2,
            Name = "group 2",
            PasswordHash = "123"
        };
        Assert.True(res.Count() == 2);
    }
    [Fact]
    public void GetGroupUsers()
    {
        var res = _userGroupManager.GetGroupUsers(1).Result;
        var u1 = new Server.User()
        {
            Id = 1,
            Name = "vlad",
            PasswordHash = "",
            LoginHash = ""
        };
        var u2 = new Server.User()
        {
            Id = 2,
            Name = "dima",
            PasswordHash = "",
            LoginHash = ""
        };
        Assert.True(res.Count() == 2);
    }

    [Fact]
    public void NoUserGetUserGroups_Fails()
    {
        var res = _userGroupManager.GetUserGroups(34).Result;
        Assert.Empty(res);
    }

    [Fact]
    public void NoGroupsGetUserGroups()
    {
        var res = _userGroupManager.GetUserGroups(3).Result;
        Assert.Empty(res);
    }

    [Fact]
    public void NoGroupGetGroupUsers()
    {
        var res = _userGroupManager.GetGroupUsers(34).Result;
        Assert.Empty(res);
    }

    [Fact]
    public void NoUserssGetGroupUsers_Fails()
    {
        var res = _userGroupManager.GetUserGroups(3).Result;
        Assert.Empty(res);
    }
}