using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using BCrypt.Net;

namespace backend.Services
{
    public class AuthService
    {
        private readonly MongoDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(MongoDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<Planner> Register(string email, string name, string password)
    {
        var planner = new Planner
        {
            Email = email,
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        await _db.Planners.InsertOneAsync(planner);
        return planner;
    }

    public async Task<string> Login(string email, string password)
    {
        var planner = await _db.Planners.Find(p => p.Email == email).FirstOrDefaultAsync();
        if (planner == null || !BCrypt.Net.BCrypt.Verify(password, planner.PasswordHash))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, planner.Id),
                new Claim(ClaimTypes.Email, planner.Email),
                new Claim(ClaimTypes.Name, planner.Name)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }
}
}