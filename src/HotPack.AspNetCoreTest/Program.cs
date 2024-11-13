using HotPack.App;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (IWebHostEnvironment env) =>
{
    Globals.Instance = new Globals(env.ContentRootPath);

    var path = Globals.Instance.ApplicationPath;
    var version = Globals.Instance.ApplicationVersion;
    var name = Globals.Instance.ApplicationName;

});

app.Run();
