//
using static AppInstance;

var builder = CreateHostBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureServices(DebugConfiguration);
App = builder.Build();

App.UseSwagger();
App.UseSwaggerUI();
App.UseAuthorization();
App.UseHttpsRedirection();
App.MapControllers();

App.Run();
