using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Talkative.Application.Interceptors;
using Talkative.Application.Mutations;
using Talkative.Application.Queries;
using Talkative.Application.Subscriptions;
using Talkative.Domain.Models;
using Talkative.Infrastructure.Context;
using Talkative.Web.Modules.Identity;
using Talkative.Web.Mounts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var settings = new Settings();
builder.Configuration.Bind("Settings", settings);
builder.Services.AddSingleton(settings);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddControllersWithViews();

builder.Services.AddRepositories();

builder.Services.AddServices();

builder.Services.AddNpgsql<ApplicationContext>(builder.Configuration.GetConnectionString("PostgreSQL"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.BearerKey)),
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidIssuer = settings.Issuer,
        ValidateIssuer = true
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions()
    /*.AddSocketSessionInterceptor<SocketSessionInterceptor>()*/
    .AddFiltering()
    .AddSorting()
    .AddAuthorization()
    .ModifyRequestOptions(x => x.IncludeExceptionDetails = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(b => b
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

// REST API
app.AddIdentityEndpoint();

// GraphQL
app.MapGraphQL();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();