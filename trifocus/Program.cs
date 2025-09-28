using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

// Configure Azure AD Authentication
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

// Add authorization policies for roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("admin"));
    options.AddPolicy("RequireAthleteRole", policy =>
        policy.RequireRole("athlete"));
});

// Configure Swagger with OAuth
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Trifocus API", Version = "v1" });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = builder.Configuration["AzureAd:Scopes"]?.Split(' ').ToDictionary(scope => scope, _ => string.Empty)
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            builder.Configuration["AzureAd:Scopes"]?.Split(' ') ?? Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trifocus API V1");
        c.RoutePrefix = "swagger";
        c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        c.OAuthScopes(builder.Configuration["AzureAd:Scopes"]?.Split(' ') ?? Array.Empty<string>());
    });
}

// Enable CORS
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
