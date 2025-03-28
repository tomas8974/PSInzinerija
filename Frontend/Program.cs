using Frontend.Components;
using Frontend.Services;
using PSInzinerija1.Shared.Data.Models.Stats;

using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var configuration = builder.Configuration;

string backendAddress = configuration.GetValue<string>("BackendAddress") ?? throw new InvalidOperationException("BackendAddress is missing from configuration");
builder.Services.AddHttpClient("BackendApi", options =>
{
    options.BaseAddress = new Uri(backendAddress);
    options.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
})
.AddHeaderPropagation();

builder.Services.AddHeaderPropagation(options =>
{
    options.Headers.Add("Cookie");
});

builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<HighScoreAPIService>();
builder.Services.AddScoped<WordListAPIService>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();
builder.Services.AddScoped<StatsAPIService<VisualMemoryStats>>();
builder.Services.AddScoped<StatsAPIService<SimonSaysStats>>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseHeaderPropagation();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
