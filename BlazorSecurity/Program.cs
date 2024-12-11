using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text.Json;
using BlazorSecurity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorSecurity.Components;
using BlazorSecurity.Components.Account;
using BlazorSecurity.Data;
using BlazorSecurity.Encryption;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

KeyManager.InitializeKeys();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddDataProtection();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var profileName = GetActiveProfile();
var connectionStringKey = "DefaultConnection";
var cprConnectionString = builder.Configuration.GetConnectionString("CprConnection") ??
                            throw new InvalidOperationException($"CPR Connection string not found.");

if (profileName == "https:test")
{
    connectionStringKey = "MockConnection";
}

var connectionString = builder.Configuration.GetConnectionString(connectionStringKey) ??
                       throw new InvalidOperationException($"Connection string {connectionStringKey} not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<CprDbContext>(options =>
    options.UseSqlite(cprConnectionString));

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
    options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());
    
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole(UserRoles.Admin));
});

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<Hasher>();
builder.Services.AddSingleton<AsymmetricEncryptionHandler>();
builder.Services.AddSingleton<SymmetricEncryptionHandler>();
builder.Services.AddSingleton<KeyManager>();

if (!IsLinux())
{
    var devCertFolderPath = IsOsx() ? ".aspnet/dev-certs/https" : ".aspnet/https";

    var devCertsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), devCertFolderPath);
    var devCert = Path.GetFullPath("BlazorSecurity.pfx", devCertsFolder);
    builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Path").Value = devCert;
    
    var kestrelPassword = builder.Configuration.GetValue<string>("KestrelCertificatePassword");

    builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Password").Value = kestrelPassword;

    builder.WebHost.UseKestrel((context, serverOptions) =>
    {
        serverOptions.Configure(context.Configuration.GetSection("Kestrel"))
            .Endpoint("HTTPS", listenOptions => { listenOptions.HttpsOptions.SslProtocols = SslProtocols.Tls12; });
    });
}

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

bool IsOsx()
{
    return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
}

bool IsLinux()
{
    return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
}

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

