using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services; 
using SOUPI.Components;
using SOUPI.Handlers;
using SOUPI.Handlers.Interfaces; 
using SOUPICore;
using SOUPICore.Services;
using SOUPICore.Services.Interfaces; 
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;


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
                if (context.TokenResponse.Response.RootElement.TryGetProperty("installation_id", out var id))
                {
                    context.Identity?.AddClaim(new Claim("github_installation_id", id.ToString()));
                }

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

builder.Services.AddDbContextFactory<SoupiDbContext>(
    options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => { b.MigrationsAssembly("SOUPI"); })
        .UseLazyLoadingProxies()
    );

builder.Services.AddHttpClient();

builder.Services.AddTransient<IGitHubRequestHandler, GitHubRequestHandler>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<GitHubRequestHandler>>();
    var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>(); 
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>(); 
    var keyGenService = sp.GetRequiredService<IKeyGenService>();  
    var devtunnelUrl = builder.Configuration["VS_TUNNEL_URL"];
    var clientId = builder.Configuration["Github:ClientId"];
    var clientSecret = builder.Configuration["Github:ClientSecret"];

    return new GitHubRequestHandler(logger, contextAccessor, httpClientFactory, keyGenService, devtunnelUrl, clientId, clientSecret); 
}); 
builder.Services.AddTransient<IUserRequestHandler, UserRequestHandler>();
builder.Services.AddTransient<IProjectRequestHandler, ProjectRequestHandler>();
builder.Services.AddTransient<ITeamMemberRequestHandler, TeamMemberRequestHandler>();
builder.Services.AddTransient<IJobRequestHandler, JobRequestHandler>(); 
builder.Services.AddTransient<IJobSequenceRequestHandler, JobSequenceRequestHandler>(); 
builder.Services.AddTransient<INotificationRequestHandler, NotificationRequestHandler>();
builder.Services.AddTransient<IAssignmentRequestHandler, AssignmentRequestHandler>();
builder.Services.AddTransient<IActivityRequestHandler, ActivityRequestHandler>(); 

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddTransient<ITeamMemberService, TeamMemberService>(); 
builder.Services.AddTransient<IJobService, JobService>(); 
builder.Services.AddTransient<IJobSequenceService, JobSequenceService>(); 
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IAssignmentService, AssignmentService>();
builder.Services.AddTransient<IActivityService, ActivityService>(); 
builder.Services.AddTransient<IKeyGenService, KeyGenService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KeyGenService>>();
    var masterKey = builder.Configuration["masterKey"]; 

    return new KeyGenService(logger, masterKey);
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

var options = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor 
                     | ForwardedHeaders.XForwardedProto 
                     | ForwardedHeaders.XForwardedHost 
};

app.UseForwardedHeaders(options);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers(); 
app.MapRazorPages(); 
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
