using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GitHubRepoSearchApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for the client.
        /// </summary>
        /// <returns>A JSON object containing the generated JWT token.</returns>
        [HttpGet("token")]
        public IActionResult GetToken()
        {
            try
            {
                // Retrieve the JWT secret key from the configuration.
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    return StatusCode(500, "JWT Key is not configured. Please set 'Jwt:Key' in appsettings.json.");
                }

                // Retrieve the token expiration time from the configuration.
                var expiresInMinutesConfig = _configuration["Jwt:ExpiresInMinutes"];
                if (string.IsNullOrEmpty(expiresInMinutesConfig) || !int.TryParse(expiresInMinutesConfig, out var expiresInMinutes))
                {
                    expiresInMinutes = 60; // Default expiration time in minutes.
                }

                // Define the claims for the JWT token.
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "TestUser"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                };

                // Create signing credentials using the secret key.
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Generate the JWT token with specified claims and expiration time.
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                    signingCredentials: creds
                );

                // Return the generated token as a response.
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            catch (Exception ex)
            {
                // Log the exception and return a server error response.
                Console.Error.WriteLine($"Error generating token: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while generating the token.");
            }
        }
    }
}
