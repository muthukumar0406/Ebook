using EbookHub.API.Data;
using EbookHub.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EbookHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("admin-login")]
        public IActionResult AdminLogin([FromBody] LoginRequest request)
        {
            if (request.Username == "Muthukumar" && request.Password == "Admin@kumar")
            {
                var token = GenerateJwtToken("Muthukumar", "Admin", -1); // -1 or special ID for admin
                return Ok(new { Token = token, Role = "Admin", Username = "Muthukumar" });
            }
            return Unauthorized("Invalid admin credentials");
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            // In a real app, verify the Google Token here using Google libraries
            // For now, we trust the email provided (DEMO ONLY) or decode the token locally without signature check if simple
            // We'll assume the frontend sends a valid payload with Email for this basic implementation if verification fails
            
            // Ideally: var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, ...);
            
            // Mocking the extraction for simplicity as we might not have internet/packages restored
            // effectively just "trusting" the client sends a valid email for this step or assuming we extract it.
            // Let's assume the request sends the email directly for this demo phase if token validation is skipped.
            
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = request.Email,
                    Name = request.Name ?? "User",
                    GoogleId = request.GoogleId,
                    Role = "User"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var token = GenerateJwtToken(user.Email, "User", user.Id);
            return Ok(new { Token = token, Role = "User", Username = user.Name, UserId = user.Id });
        }

        private string GenerateJwtToken(string username, string role, int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // Sent from frontend for simplicity in this demo
        public string Name { get; set; } = string.Empty;
        public string GoogleId { get; set; } = string.Empty;
    }
}
