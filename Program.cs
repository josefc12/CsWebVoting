using cs_web_voting.Data;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CsWebVotingDbContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                );
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddMvc()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // or use a custom naming policy
    });

builder.Services.AddHostedService<Counter>();

var app = builder.Build();
//44471 in dev This is needed only for dev
app.UseCors(options => options.WithOrigins("https://localhost:44471").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");
app.MapHub<RoomHub>("/RoomHub");

app.Run();
