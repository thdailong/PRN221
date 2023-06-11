using Fatima.DAM;
using Fatima.Model;
using Fatima.Services;
using Fatima.Utils;
using Serilog;
using Serilog.Formatting.Json;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(Identity.Schema).AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/error/accessDenied";
    options.LogoutPath = "/logout";
});

builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("GuestOnly", policy => policy.RequireAssertion(ctx => ctx.User.Identity?.IsAuthenticated != true));
    cfg.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Email, "admin@fatima.com"));
});

// Set this for printting special chars to the console
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Get the app config
var cfg = builder.Configuration;

// Create Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(), "logs/.log",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
// Register serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Initialize services client
await Clients.Init(cfg);

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.Use(async (ctx, next) =>
{
    await next();
    if (ctx.Response.StatusCode == 404) ctx.Response.Redirect("/error/notfound");
});

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (ctx, next) =>
{
    var account = ctx.Items["Account"] = ctx.User.Identity?.IsAuthenticated == true ? await AccountDAM.Get(ctx.User.FindFirstValue(ClaimTypes.Email)) : null;
    if (account != null) await AccountDAM.UpdateActive((account as Account)!.Email);
    await next();
});

app.MapControllers();
app.MapRazorPages();
app.MapHub<WS>("/_ws");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Error while running application.");
}
finally
{
    // Close all clients connection
    Clients.CloseAll();
}
