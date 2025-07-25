﻿using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PTM.Application.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace PTM.Application.Commands
{
    public record LoginUserCommand(LoginRequestDto Dto) : IRequest<ApiResponse<LoginResponseDto>>;
    public class LoginUserCommandHandler(IEntityCommiter commiter, IConfiguration config, ILogger<LoginUserCommandHandler> logger) : IRequestHandler<LoginUserCommand, ApiResponse<LoginResponseDto>>
    {
        public async Task<ApiResponse<LoginResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Login attempt for user: {Username}", request.Dto.UserName);
            var userRequest = await commiter.Users.GetAsync(
                filter: x => x.Username == request.Dto.UserName,
                include: u => u.Include(i => i.Role));

            if (userRequest.IsSuccess is false || userRequest.Data is null)
            {
                logger.LogWarning("Login failed for user: {Username}. Reason: {Message}", request.Dto.UserName, userRequest.Message ?? "User Name or password is wrong");
                return ApiResponse<LoginResponseDto>.Failure(HttpStatusCode.Unauthorized, userRequest.Message ?? "User Name or password is wrong");
            }

            var user = userRequest.Data;

            var hasher = new PasswordHasher();
            var result = hasher.VerifyHashedPassword(user.PasswordHash, request.Dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                logger.LogWarning("Login failed for user: {Username}. Reason: Incorrect password.", request.Dto.UserName);
                return ApiResponse<LoginResponseDto>.Failure(HttpStatusCode.Unauthorized, "User Name or password is wrong");
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var loginResult = new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                Role = user.Role.Name
            };
            logger.LogInformation("User {Username} logged in successfully with role {Role}", user.Username, user.Role.Name);
            return ApiResponse<LoginResponseDto>.Success(loginResult);
        }
    }
}
