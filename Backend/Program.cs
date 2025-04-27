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
using Backend.Interfaces;
using Backend.Wrappers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpClient();

string frontendAddress = configuration.GetValue<string>("FrontendAddress") ?? throw new InvalidOperationException("FrontendAddress is missing from configuration");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendAddress, "http://localhost")
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

builder.Services.AddSingleton<ISmtpClient>(sp =>
{
    var smtpHost = builder.Configuration["SmtpSettings:Host"];
    var smtpPort = int.Parse(builder.Configuration["SmtpSettings:Port"]);
    var smtpUsername = builder.Configuration["SmtpSettings:Username"];
    var smtpPassword = builder.Configuration["SmtpSettings:Password"];
    var useSsl = bool.Parse(builder.Configuration["SmtpSettings:UseSsl"]);

    return new SmtpClientWrapper(smtpHost, smtpPort, smtpUsername, smtpPassword, useSsl);
});

builder.Services.AddSingleton<IEmailSender<User>, EmailSendingService>(sp =>
{
    var smtpClient = sp.GetRequiredService<ISmtpClient>();
    var fromEmail = builder.Configuration["SmtpSettings:FromEmail"];
    var fromName = builder.Configuration["SmtpSettings:FromName"];

    return new EmailSendingService(smtpClient, fromEmail, fromName);
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context?.Database.Migrate();
}

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

app.Run();

public partial class Program { }
