using System.Text.Json;
using BlazorSecurity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorSecurity.Components;
using BlazorSecurity.Components.Account;
using BlazorSecurity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var profileName = GetActiveProfile();
var connectionStringKey = "DefaultConnection";

if (profileName == "https:test")
{
    connectionStringKey = "MockConnection";
}

var connectionString = builder.Configuration.GetConnectionString(connectionStringKey) ??
                       throw new InvalidOperationException($"Connection string {connectionStringKey} not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole(UserRoles.Admin));
});

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
return;

string GetActiveProfile()
{
    var launchSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), 
        "Properties", 
        "launchSettings.json");
    
    if (!File.Exists(launchSettingsPath))
        return "unknown";

    var launchSettings = JsonDocument.Parse(File.ReadAllText(launchSettingsPath));
    var profiles = launchSettings.RootElement.GetProperty("profiles");
    
    var currentUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "";
    
    foreach (var profile in profiles.EnumerateObject())
    {
        var applicationUrl = profile.Value.TryGetProperty("applicationUrl", out var urlProperty) 
            ? urlProperty.GetString() 
            : "";
            
        if (applicationUrl != null && currentUrls.Contains(applicationUrl.Split(';')[0]))
        {
            return profile.Name;
        }
    }
    
    return "unknown";
}