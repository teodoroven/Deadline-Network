using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Server.App.Controllers;
using Server.App.Db.Contexts;
namespace Tests;
// Hello f*ckig world
public class DisciplineCRUDControllerTests
{
    ApplicationDbContext _db;
    private DisciplineCRUDController _crud;

    public DisciplineCRUDControllerTests()
    {
        // Create an in-memory database with the fake data
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _db = new ApplicationDbContext(options);
        _crud = new DisciplineCRUDController(_db);

        _db.Users.Add(new Server.User
        {
            Name = "vlad",
            Id = 1
        });
        _db.Users.Add(new Server.User
        {
            Name = "dima",
            Id = 2
        });
        _db.Users.Add(new Server.User
        {
            Name = "sasha",
            Id = 3
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

    }
    [Fact]
    public void AddDiscipline()
    {
        var res = _crud.Add(1,1,"descipline1","comment").Result;
        Assert.Equal(res.StatusCode,StatusCodes.Status200OK);
        Assert.True(_db.Disciplines.Any(d=>d.Name=="descipline1"));
    }
    [Fact]
    public void IllegalAddDiscipline_Fails()
    {
        var res = _crud.Add(2,1,"descipline2","comment").Result;
        Assert.Equal(res.StatusCode,StatusCodes.Status403Forbidden);
        Assert.False(_db.Disciplines.Any(d=>d.Name=="descipline2"));
    }
    [Fact]
    public void NoUserAddDiscipline_Fails()
    {
        var res = _crud.Add(20,1,"descipline2","comment").Result;
        Assert.Equal(res.StatusCode,StatusCodes.Status401Unauthorized);
        Assert.False(_db.Disciplines.Any(d=>d.Name=="descipline2"));
    }
    [Fact]
    public void NoGroupAddDiscipline_Fails()
    {
        var res = _crud.Add(1,10,"descipline2","comment").Result;
        Assert.Equal(res.StatusCode,StatusCodes.Status401Unauthorized);
        Assert.False(_db.Disciplines.Any(d=>d.Name=="descipline2"));
    }
}