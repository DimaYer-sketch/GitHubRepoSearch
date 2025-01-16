using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Register services for dependency injection
    builder.Services.AddControllers(options =>
    {
        // Add a global logging filter for incoming requests
        options.Filters.Add<Server.Filters.LogRequestAttribute>();
    });

    // Register HttpClient service for making HTTP requests
    builder.Services.AddHttpClient();

    // Configure CORS to allow cross-origin requests
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Replace with your trusted domains
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Retrieve the JWT key from configuration
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("JWT Key is not configured. Please set 'Jwt:Key' in appsettings.json.");
    }

    // Configure JWT authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero, // Eliminate clock skew for precise token expiration validation
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

    // Register authorization services
    builder.Services.AddAuthorization();

    // Register Swagger for API documentation
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "GitHubRepoSearchAPI", Version = "v1" });

        // Configure Swagger to use JWT Authentication
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Application initialization failed: {ex.Message}");
    throw;
}

var app = builder.Build();

// Enable Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline
app.UseCors(); // Enable Cross-Origin Resource Sharing
app.UseMiddleware<Server.Middleware.ErrorHandlingMiddleware>(); // Global error handling middleware
app.UseAuthentication(); // Enable JWT authentication
app.UseAuthorization(); // Enable authorization policies
app.MapControllers(); // Map endpoints to controllers

app.Run(); // Start the application
