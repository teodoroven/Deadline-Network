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
        var conf = context.Configuration;
        services.AddDbContextPool<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(conf.GetConnectionString("Db"))
                .UsePostgreSqlTriggers();
        });

        if (EF.IsDesignTime) return;
        AddSignalR(services, conf);
        
    }
    public static WebApplicationBuilder CreateHostBuilder(string[] args){
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json",optional:true,reloadOnChange:true);
        builder.Configuration.AddJsonFile("secrets.json",optional:true,reloadOnChange:true);
        return builder;
    }
    static void AddSignalR(IServiceCollection services, IConfiguration conf)
    {
        //нужно добавить в json файл secrets.json данные о redis-сервере.
        //он нужен для синхронизации signal-r приложений между несколькими серверами
        // "Redis":{
        //    "Host":"123.123.123.123",
        //    "Port":1000,
        //    "Password":"password"
        // }
        var url="";
        if(conf["Redis"] is not null)
            url = $"{conf["Redis:Host"]}:{conf["Redis:Port"]}, password={conf["Redis:Password"]}";
        
        services.AddSignalR().AddStackExchangeRedis(url);
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
    }
}
