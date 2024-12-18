using System.Text;
using Authentication.Models;
using Authentication.Repositories;
using Authentication.Services.Concrete;
using Authentication.Services.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Important: Configuring the database connection. You can choose the database you want.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(connectionString));

// Important: Setting up Identity with password requirements.
builder.Services.AddIdentity<User, IdentityRole>
(
    options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
    }
).AddEntityFrameworkStores<Context>();

// Not Important: Configuring AutoMapper.
// Mapper Injection
builder.Services.AddAutoMapper(typeof(Program));

// Important: Configuring JWT authentication with token validation parameters.
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
        options.DefaultForbidScheme =
        options.DefaultScheme =
        options.DefaultSignInScheme =
        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(
    options => options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey
        (
            Encoding.UTF8.GetBytes(s: builder.Configuration["JWT:SigninKey"]!)
        ),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    }
);

// Important: Adding authorization services.
builder.Services.AddAuthorization();

// Not Important: Adding MVC services;
builder.Services.AddControllersWithViews();

// Not Important: Adding endpoint API explorer for Swagger.
builder.Services.AddEndpointsApiExplorer();

// Optional: Configuring Swagger for API documentation.
// This are for testing tokens on authorization works.
// Swagger Configrations
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Important: Adding custom services for dependency injection.
// You can use your services directly. I chose Manager design.
// My Services
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Not Important: Configuring the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Not Important: Configuring HSTS for production.
    app.UseHsts();
}

// Important: Enabling Swagger and Swagger UI.
app.UseSwagger();
app.UseSwaggerUI();

// Important: Configuring middleware for HTTPS redirection and static files.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Important: Adding authentication and authorization middleware.
// Authentication Middleware
app.UseAuthentication();
app.UseAuthorization();

// Important: Mapping controller routes.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();