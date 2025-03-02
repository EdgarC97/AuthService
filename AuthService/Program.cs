using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AuthService.Data;
using AuthService.Helpers;
using AuthService.Repositories;
using AuthService.Repositories.Interfaces;
using AuthService.Services;
using AuthService.Services.Interfaces;
using AuthService.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables based on the current environment.
// Retrieves the ASPNETCORE_ENVIRONMENT variable; defaults to "Development" if not set.
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
if (env == "Development")
    // Load local environment variables for development.
    Env.Load(".env.local");
else
    // Load production environment variables.
    Env.Load(".env.production");

// Configure JWT settings by reading environment variables.
// Create a JwtSettings object with properties read from environment variables.
var jwtSettings = new JwtSettings
{
    Secret = Environment.GetEnvironmentVariable("JWT_SECRET"),
    ExpirationInMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION")),
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
};

// Register the JWT settings as a singleton in the dependency injection container.
builder.Services.AddSingleton(jwtSettings);

// Configure the SQL Server connection using the connection string from environment variables.
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories and services to apply SOLID principles.
// This makes them available for dependency injection across the application.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();

// Configure AutoMapper with the defined mapping profile.
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure JWT authentication scheme.
builder.Services.AddAuthentication(options =>
{
    // Set the default authentication scheme to JwtBearer.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configure token validation parameters for JWT.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Ensure the token issuer is valid.
        ValidateAudience = true, // Ensure the token audience is valid.
        ValidateLifetime = true, // Ensure the token has not expired.
        ValidateIssuerSigningKey = true, // Validate the signing key.
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

// Register controllers to handle HTTP requests.
builder.Services.AddControllers();
// Configure Swagger for API documentation and testing.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger middleware for API documentation.
app.UseSwagger();
app.UseSwaggerUI();

// Enable authentication and authorization middlewares.
app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints to handle requests.
app.MapControllers();

// Automatic migration and creation of the database (only in development).
if (app.Environment.IsDevelopment())
{
    // Create a service scope to retrieve the ApplicationDbContext.
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // Ensure the database is created. This can be replaced with migrations in production.
        context.Database.EnsureCreated();
    }
}

// Run the application.
app.Run();
