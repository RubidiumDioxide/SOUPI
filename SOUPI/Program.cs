using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth; 
using MudBlazor;
using MudBlazor.Services;
using SOUPI.Components;
using SOUPICore.Services;
using SOUPI;
using SOUPICore;
using System.Net.Http.Headers;
using System.Security.Claims; 
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SOUPI.Handlers.Interfaces; 
using SOUPI.Handlers;
using SOUPICore.Services.Interfaces;
using Microsoft.AspNetCore.Components; 


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); 
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState(); 

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

builder.Services.AddDbContext<SoupiDbContext>(
    options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => { b.MigrationsAssembly("SOUPI"); })
        .UseLazyLoadingProxies()
    );

builder.Services.AddSingleton<AuthHttpClientFactory>();
builder.Services.AddScoped<HttpClient>(sp =>
{
    var navManager = sp.GetRequiredService<NavigationManager>(); 
    var factory = sp.GetRequiredService<AuthHttpClientFactory>();
    var client = factory.CreateClient(new Uri(navManager.BaseUri));

    return client;
});

builder.Services.AddScoped<IGithubRequestHandler, GithubRequestHandler>(); 
builder.Services.AddScoped<IUserRequestHandler, UserRequestHandler>();
builder.Services.AddScoped<IProjectRequestHandler, ProjectRequestHandler>();
builder.Services.AddScoped<ITeamMemberRequestHandler, TeamMemberRequestHandler>();
builder.Services.AddScoped<IJobRequestHandler, JobRequestHandler>(); 
builder.Services.AddScoped<IJobSequenceRequestHandler, JobSequenceRequestHandler>(); 
builder.Services.AddScoped<INotificationRequestHandler, NotificationRequestHandler>();
builder.Services.AddScoped<IAssignmentRequestHandler, AssignmentRequestHandler>(); 

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>(); 
builder.Services.AddScoped<IJobService, JobService>(); 
builder.Services.AddScoped<IJobSequenceService, JobSequenceService>(); 
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>(); 

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

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
