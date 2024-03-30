using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
using Server;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tests;

public class DatabaseFixure: IDisposable
{
    public ApplicationDbContext Db { get; private set; }
    void AddUser(int id, string Name, string login, string password){
        var salt = HashService.GenerateSalt();
        var hash = new HashService();
        Db.Users.Add(new User
        {
            Name = Name,
            Id = id,
            LoginHash=  hash.Hash(login),
            PasswordHash=hash.Hash(password,salt),
            PasswordSalt=salt
        });
    }
    public DatabaseFixure()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase", new InMemoryDatabaseRoot())
            .Options;
        var hash = new HashService();
        Db = new ApplicationDbContext(options);
        AddUser(1,"vlad","as","123");
        AddUser(2,"dima","sd","123");
        AddUser(3,"sasha","asd","123");
      
        Db.Groups.Add(new Group{
            Id=1,
            Name="group 1",
            PasswordHash=hash.Hash("123")
        });
        Db.Groups.Add(new Group{
            Id=2,
            Name="group 2",
            PasswordHash=hash.Hash("123")
        });
        Db.UserGroups.Add(new UserGroup{
            GroupId=1,
            UserId=1,
            IsOwner=true
        });
        Db.UserGroups.Add(new UserGroup{
            GroupId=1,
            UserId=2,
            IsOwner=false
        });
        Db.UserGroups.Add(new UserGroup{
            GroupId=2,
            UserId=1,
            IsOwner=false
        });
        Db.SaveChanges();
    }
    public void Dispose()
    {
        Db.Database.EnsureDeleted();
    }
}