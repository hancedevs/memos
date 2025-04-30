// Endpoints/PlannerEndpoints.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Dto;
using backend;

public static class PlannerEndpoints
{
    public static void MapPlannerEndpoints(this WebApplication app)
    {
        app.MapPost("/api/planners", async ([FromBody] PlannerCreateDto dto, AuthService authService) =>
        {
            try
            {
                var plannerId = await authService.Register(dto);
                return Results.Ok(new { PlannerId = plannerId });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).AddEndpointFilter(async (context, next) =>
        {
            var dto = context.GetArgument<PlannerCreateDto>(0);
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Name is required.");
            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(dto.Email))
                errors.Add("Invalid email format.");
            if (dto.Password.Length < 8 || !System.Text.RegularExpressions.Regex.IsMatch(dto.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
                errors.Add("Password must be at least 8 characters and contain one uppercase letter, one lowercase letter, and one number.");

            return errors.Any() ? Results.BadRequest(new { Errors = errors }) : await next(context);
        });

        app.MapGet("/api/planners", async (MemoDbContext context) =>
        {
            var planners = await context.Planners.Select(p => new { p.Id, p.Name, p.Email }).ToListAsync();
            return Results.Ok(planners);
        });
    }
}