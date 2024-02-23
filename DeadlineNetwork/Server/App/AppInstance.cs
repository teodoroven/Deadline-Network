using Microsoft.EntityFrameworkCore;
using Laraue.EfCoreTriggers.PostgreSql.Extensions;
using Server.App.Db.Contexts;
// using Server.App.Db.Contexts;

/// <summary>
/// Place that contains reference to global application running, so it could be accessed from everywhere
/// </summary>
public static class AppInstance
{
    public static WebApplication? App { get; set; }
    public static IServiceProvider? Services => App?.Services;
    public static void DebugConfiguration(HostBuilderContext context, IServiceCollection services)
    {
        services.AddDbContextPool<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(context.Configuration.GetConnectionString("Db"))
                .UsePostgreSqlTriggers();
        });
        
        if(EF.IsDesignTime) return;
        services.AddScoped<IApplicationDbContext,ApplicationDbContext>();
    }
    public static WebApplicationBuilder CreateHostBuilder(string[] args){
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json",optional:true,reloadOnChange:true);
        builder.Configuration.AddJsonFile("secrets.json",optional:true,reloadOnChange:true);
        return builder;
    }
}
