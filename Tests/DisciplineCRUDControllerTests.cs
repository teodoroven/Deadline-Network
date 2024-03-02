using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
namespace Tests;
// Hello f*ckig world
public class DesciplineCRUDControllerTests
{
    public DesciplineCRUDControllerTests()
    {
        // Create an in-memory database with the fake data
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        
        using var context = new ApplicationDbContext(options);
        
    }
    [Fact]
    public void Test1()
    {

    }
}