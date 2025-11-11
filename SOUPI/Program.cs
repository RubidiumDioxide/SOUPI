using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth; 
using MudBlazor;
using MudBlazor.Services;
using SOUPI.Components;
using SOUPI.Services;
using SOUPI;
using System.Net.Http.Headers;
using System.Security.Claims; 
using System.Text.Json;
using Microsoft.EntityFrameworkCore; 


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); 
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Github";
})
    .AddCookie()
    .AddOAuth("Github", options =>
    {
        options.ClientId = builder.Configuration["Github:ClientId"];
        options.ClientSecret = builder.Configuration["Github:ClientSecret"];
        options.Scope.Add("repo");
        options.CallbackPath = new PathString("/signin-github");
        options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        options.TokenEndpoint = "https://github.com/login/oauth/access_token";
        options.UserInformationEndpoint = "https://api.github.com/user";
        options.SaveTokens = true;
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey("urn:github:login", "login");
        options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
        options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();
                var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(json.RootElement);
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<SoupiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Logging.AddConsole();
builder.Services.AddScoped<IGithubService, GithubService>(); 
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
builder.Services.AddSingleton<AuthHttpClientFactory>(); 
builder.Services.AddScoped<IUserService>(sp =>
{
    var factory = sp.GetRequiredService<AuthHttpClientFactory>();
    var logger = sp.GetRequiredService<ILogger<UserService>>();
    var baseAddress = new Uri(builder.Configuration["Urls"]);
    var client = factory.CreateClient(baseAddress);

    return new UserService(logger, client);
});
builder.Services.AddScoped<IProjectService>(sp =>
{
    var factory = sp.GetRequiredService<AuthHttpClientFactory>(); 
    var logger = sp.GetRequiredService<ILogger<ProjectService>>();
    var baseAddress = new Uri(builder.Configuration["Urls"]);
    var client = factory.CreateClient(baseAddress);

    return new ProjectService(logger, client);
}); 

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled; 
}); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); 

app.UseRouting();

app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers(); 
app.MapRazorPages(); 
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
