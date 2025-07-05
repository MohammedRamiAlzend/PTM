using FluentValidation;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PTM.Application.DTOs.AuthDTOs;
using PTM.Domain.Entities;
using System.Net;
using Microsoft.Extensions.Logging;

namespace PTM.Application.Commands;

public record RegisterUserCommand(RegisterRequestDto Dto) : IRequest<ApiResponse<RegisterResponseDto>>;
public class RegisterUserCommandHandler(
    IEntityCommiter commiter, 
    IConfiguration config,
    IValidator<RegisterRequestDto> validator,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, ApiResponse<RegisterResponseDto>>
{
    public async Task<ApiResponse<RegisterResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registration attempt for user: {Username}", request.Dto.Username);
        var validateResult = await validator.ValidateAsync(request.Dto);
        if (validateResult.IsValid is false) 
        {
            logger.LogWarning("Registration validation failed for user: {Username}. Errors: {@Errors}", request.Dto.Username, validateResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<RegisterResponseDto>.Failure(HttpStatusCode.BadRequest,[.. validateResult.Errors.Select(x => x.ErrorMessage)]);
        }
        
        var getRolesResult = await commiter.Roles.GetAllAsync(include: i => i.Include(x => x.Users));

        if (getRolesResult.IsSuccess is false || (getRolesResult.Data ?? []).Count == 0)
        {
            logger.LogError("Failed to retrieve roles or no roles seeded to database for user {Username} registration.", request.Dto.Username);
            return ApiResponse<RegisterResponseDto>.Failure(HttpStatusCode.InternalServerError, "there is no roles seeded to database ...");
        }
        var getRoles = getRolesResult.Data!;
        if (getRoles.Any(x => x.Name.ToLower() == request.Dto.RoleName.ToLower()) is false)
        {
            logger.LogWarning("Role {RoleName} not found for user {Username} registration.", request.Dto.RoleName, request.Dto.Username);
            return ApiResponse<RegisterResponseDto>.Failure(HttpStatusCode.BadRequest, $"there is no roles named {request.Dto.RoleName} ...");
        }
        var user = new User()
        {
            PasswordHash = new PasswordHasher().HashPassword(request.Dto.Password),
            Username = request.Dto.Username,
            Role = getRoles.Where(x => x.Name.ToLower() == request.Dto.RoleName.ToLower()).First()
        };

        try
        {
            var addingResult = await commiter.Users.AddAsync(user);
            var commitResult = await commiter.CommitAsync(cancellationToken);
            if (addingResult.IsSuccess is false || commitResult < 0)
            {
                logger.LogError("Failed to add user {Username} to the database or commit changes.", request.Dto.Username);
                throw new Exception();
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while registering the user {Username}.", request.Dto.Username);
            return ApiResponse<RegisterResponseDto>.Failure(HttpStatusCode.InternalServerError, "An error occurred while registering the user.");
        }

        logger.LogInformation("User {Username} registered successfully with Id: {UserId} and Role: {RoleName}", user.Username, user.Id, user.Role.Name);
        return ApiResponse<RegisterResponseDto>.Success(MapToRegisterResponseDto(user), HttpStatusCode.OK, "User registered successfully");
    }

    private static RegisterResponseDto MapToRegisterResponseDto(User user)
    {
        return new RegisterResponseDto
        {
            Id = user.Id,
            Username = user.Username
        };
    }
}
