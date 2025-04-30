using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
           app.MapPost("/api/auth/login", async ([FromBody] LoginRequest request, AuthService authService) =>
        {
            var token = await authService.Login(request.Email, request.Password);
            return token != null ? Results.Ok(new { Token = token }) : Results.BadRequest("Invalid email or password.");
        });
        }
    }
}