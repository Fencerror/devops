using DevOpsPollApp.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        o => o.EnableRetryOnFailure()
    ));


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DevOpsPollApp.Data.ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseStaticFiles();

app.UseRouting();
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Polls}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok("OK"));

app.MapMetrics("/metrics");

app.Run();
