using System.Reflection;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Backend.Filters;
using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;
using Backend.Services;
using PSInzinerija1.Shared.Data.Models.Stats;
using Shared.Data.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://frontend:5001/")
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddTransient<HighScoreService>();
builder.Services.AddSingleton<APITrackingService>();
builder.Services.AddScoped<APIHitCountFilter>();
builder.Services.AddScoped<WordListService>();
builder.Services.AddScoped<GameStatsService<VisualMemoryStats>>();
builder.Services.AddScoped<GameStatsService<SimonSaysStats>>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "High score API",
        Description = "An ASP.NET Core Web API for managing game high scores",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
}
app.UseSwagger();
app.UseSwaggerUI();
using (var scope = app.Services.CreateScope())
{
    var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbcontext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapSwagger();

app.MapModifiedIdentityApi<User>();
app.MapControllers();

// temp
app.MapPost("/api/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
    return TypedResults.Ok();
})
.RequireAuthorization();

// temp
app.MapGet("/api/user/info", async Task<Results<Ok<UserInfo>, ValidationProblem, NotFound>>
    (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
{
    var userManager = sp.GetRequiredService<UserManager<User>>();
    return await userManager.GetUserAsync(claimsPrincipal) is not { } user
        ? (Results<Ok<UserInfo>, ValidationProblem, NotFound>)TypedResults.NotFound()
        : TypedResults.Ok(new UserInfo(user.Email, user.UserName));
});


app.Run();

public partial class Program { }
