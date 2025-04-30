// Services/AuthService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using backend.Dto;
using backend.Models;
using backend;

public class AuthService
{
    private readonly MemoDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(MemoDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> Login(string email, string password)
    {
        var planner = await _context.Planners.FirstOrDefaultAsync(p => p.Email == email);
        if (planner == null || !BCrypt.Net.BCrypt.Verify(password, planner.Password))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, planner.Id.ToString()),
                new Claim(ClaimTypes.Email, planner.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<Guid> Register(PlannerCreateDto dto)
    {
        if (await _context.Planners.AnyAsync(p => p.Email == dto.Email))
            throw new InvalidOperationException("Email already exists.");

        var planner = new Planner
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Planners.Add(planner);
        await _context.SaveChangesAsync();
        return planner.Id;
    }
}