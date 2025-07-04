using FluentValidation;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PTM.Application.DTOs.AuthDTOs;
using PTM.Domain.Entities;
using System.Net;

namespace PTM.Application.Commands;

public record RegisterUserCommand(RegisterRequestDto Dto) : IRequest<ApiResponse<RegisterResponseDto>>;
public class RegisterUserCommandHandler(
    IEntityCommiter commiter, 
    IConfiguration config,
    IValidator<RegisterRequestDto> validator) : IRequestHandler<RegisterUserCommand, ApiResponse<RegisterResponseDto>>
{
    public async Task<ApiResponse<RegisterResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validateResult = await validator.ValidateAsync(request.Dto);
        if (validateResult.IsValid is false) 
            return ApiResponse<RegisterResponseDto>.Failure(HttpStatusCode.BadRequest,[.. validateResult.Errors.Select(x => x.ErrorMessage)]);
        
        
        var getRolesResult = await commiter.Roles.GetAllAsync(include: i => i.Include(x => x.Users));

        if (getRolesResult.IsSuccess is false || (getRolesResult.Data ?? []).Count == 0)
        {
            return DbRequest<RegisterResponseDto>.Failure("there is no roles seeded to database ...");
        }
        var getRoles = getRolesResult.Data!;
        if (getRoles.Any(x => x.Name.ToLower() == request.Dto.RoleName.ToLower()) is false)
        {
            return DbRequest<RegisterResponseDto>.Failure($"there is no roles named {request.Dto.RoleName} ...");
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
            var commitResult = await commiter.CommitAsync();
            if (addingResult.IsSuccess is false || commitResult < 0)
            {
                throw new Exception();
            }
        }
        catch
        {
            return DbRequest<RegisterResponseDto>.Failure("An error occurred while registering the user.");
        }

        return DbRequest<RegisterResponseDto>.Success(MapToRegisterResponseDto(user), "User registered successfully");
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
