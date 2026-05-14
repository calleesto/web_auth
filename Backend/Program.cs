using System.Text;
using Backend;
using Backend.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using UserService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["ApiSettings:Issuer"],
            ValidAudience = builder.Configuration["ApiSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:Secret"]!))
        };
    }
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOrAdmin", policy =>
    {
        policy.Requirements.Add(new UserOrAdminRequirement());
    });
    options.AddPolicy("AdminWorkingHours", policy =>
    {
        policy.Requirements.Add(new WorkingHoursRequirement());
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.
            WithOrigins(builder.Configuration["ApiSettings:CorsOrigins"]!).
            AllowAnyHeader().
            AllowAnyMethod().
            AllowCredentials();
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, UserOrAdminHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, WorkingHoursHandler>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<Database>();

WebApplication app = builder.Build();
app.MapControllers();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();