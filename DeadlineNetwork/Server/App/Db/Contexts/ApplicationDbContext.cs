using Microsoft.EntityFrameworkCore;
namespace Server.App.Db.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}