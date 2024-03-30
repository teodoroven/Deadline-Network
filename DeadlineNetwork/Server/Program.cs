//
using System.Reflection;
using static AppInstance;

var builder = CreateHostBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(opts =>
  {
      var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

      opts.IncludeXmlComments(xmlCommentsFullPath);

      opts.AddSignalRSwaggerGen(_ =>
      {
          _.UseHubXmlCommentsSummaryAsTagDescription =  true;
          _.UseHubXmlCommentsSummaryAsTag = true;
          _.UseXmlComments(xmlCommentsFullPath);
      });
  });

builder.Host.ConfigureServices(DebugConfiguration);
App = builder.Build();

App.UseSwagger();
App.UseSwaggerUI();
App.UseAuthorization();
App.UseHttpsRedirection();
App.MapControllers();

App.Run();

// my test code