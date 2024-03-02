using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
namespace Tests;
// Hello f*ckig world
public class DisciplineCRUDControllerTests
{
    ApplicationDbContext _db;

    public DisciplineCRUDControllerTests()
    {
        // Create an in-memory database with the fake data
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        
        _db = new ApplicationDbContext(options);

    }
    [Fact]
    public void AddDiscipline()
    {
        _db.Users.Add(new Server.User{
            Name="vlad",
            Id=1
        });
        _db.Users.Add(new Server.User{
            Name="dima",
            Id=2
        });
        _db.Users.Add(new Server.User{
            Name="sasha",
            Id=3
        });
    }
}