using CMIS.Components;
using CMIS.Data;
using CMIS.Services;
using CMIS.Services.FinancialApi;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => options.DetailedErrors = true);

builder.Services.AddMudServices();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["EventApiUrl"] ?? "http://localhost:5200/") });

// Database - MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register DbContext as scoped from the factory (for pages that inject DbContext directly)
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

// Authentication & Authorization
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<DistrictEventService>();
builder.Services.AddScoped<BODEventService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();

var financialApiBaseUrl = builder.Configuration["FinancialApi:BaseUrl"]
    ?? throw new InvalidOperationException("Configuration 'FinancialApi:BaseUrl' not found.");
var financialApiHttp = builder.Services.AddHttpClient<IFinancialApiClient, FinancialApiClient>(client =>
{
    client.BaseAddress = new Uri(financialApiBaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// In Development, accept the appointment-service's untrusted dev cert so HTTPS calls
// don't fail when the cert isn't installed in the trust store.
if (builder.Environment.IsDevelopment())
{
    financialApiHttp.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
}

// Cookie-based authentication (RBAC - feature/membership)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
}).AddCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";
});

var app = builder.Build();

// Ensure database tables and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DatabaseMigrator.EnsureTablesAsync(db);
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DatabaseSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();  

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
