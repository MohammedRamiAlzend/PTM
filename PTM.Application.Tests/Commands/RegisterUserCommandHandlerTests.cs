using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.Commands;
using PTM.Application.DTOs.AuthDTOs;
using PTM.Domain.CommunicationModels;
using PTM.Domain.Entities;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IEntityCommiter> _commiterMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IValidator<RegisterRequestDto>> _validatorMock;
    private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
    private readonly RegisterUserCommandHandler _handler;
    private readonly PasswordHasher _passwordHasher;

    public RegisterUserCommandHandlerTests()
    {
        _commiterMock = new Mock<IEntityCommiter>();
        _configMock = new Mock<IConfiguration>();
        _validatorMock = new Mock<IValidator<RegisterRequestDto>>();
        _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();
        _passwordHasher = new PasswordHasher();

        _handler = new RegisterUserCommandHandler(
            _commiterMock.Object,
            _configMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var dto = new RegisterRequestDto { Username = "", Password = "", RoleName = "" };
        var command = new RegisterUserCommand(dto);

        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> {
                new("Username", "Username is required"),
                new("Password", "Password is required")
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.BadRequest);
        result.Message.Should().Contain("Username is required");
        result.Message.Should().Contain("Password is required");
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenRolesNotSeeded()
    {
        // Arrange
        var dto = new RegisterRequestDto { Username = "newuser", Password = "password123", RoleName = "User" };
        var command = new RegisterUserCommand(dto);

        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _commiterMock.Setup(c => c.Roles.GetAllAsync(
                  It.IsAny<Expression<Func<Role, bool>>?>(),
                  It.IsAny<Func<IQueryable<Role>, IIncludableQueryable<Role, object>>?>(),
                  It.IsAny<Func<IQueryable<Role>, IOrderedQueryable<Role>>?>()
            ))
            .ReturnsAsync(DbRequest<List<Role>>.Failure("No roles seeded"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("there is no roles seeded to database");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenRoleNotFound()
    {
        // Arrange
        var dto = new RegisterRequestDto { Username = "newuser", Password = "password123", RoleName = "NonExistentRole" };
        var command = new RegisterUserCommand(dto);

        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _commiterMock.Setup(c => c.Roles.GetAllAsync(
            It.IsAny<Expression<Func<Role, bool>>?>(),
            It.IsAny<Func<IQueryable<Role>, IIncludableQueryable<Role, object>>?>(),
            It.IsAny<Func<IQueryable<Role>, IOrderedQueryable<Role>>?>()))
            .ReturnsAsync(DbRequest<List<Role>>.Success(new List<Role> { new Role { Name = "User" } }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.BadRequest);
        result.Message.Should().Contain($"there is no roles named {dto.RoleName}");
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenAddUserFails()
    {
        // Arrange
        var dto = new RegisterRequestDto { Username = "newuser", Password = "password123", RoleName = "User" };
        var command = new RegisterUserCommand(dto);

        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _commiterMock.Setup(c => c.Roles.GetAllAsync(It.IsAny<Expression<Func<Role, bool>>?>(), It.IsAny<Func<IQueryable<Role>, IIncludableQueryable<Role, object>>?>(), It.IsAny<Func<IQueryable<Role>, IOrderedQueryable<Role>>?>()))
            .ReturnsAsync(DbRequest<List<Role>>.Success(new List<Role> { new Role { Name = "User" } }));

        _commiterMock.Setup(c => c.Users.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(DbRequest.Failure("Failed to add user"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("An error occurred while registering the user.");

        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRegistrationSuccessful()
    {
        // Arrange
        var dto = new RegisterRequestDto { Username = "newuser", Password = "password123", RoleName = "User" };
        var command = new RegisterUserCommand(dto);

        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _commiterMock.Setup(c => c.Roles.GetAllAsync(It.IsAny<Expression<Func<Role, bool>>?>(), It.IsAny<Func<IQueryable<Role>, IIncludableQueryable<Role, object>>?>(), It.IsAny<Func<IQueryable<Role>, IOrderedQueryable<Role>>?>()))
            .ReturnsAsync(DbRequest<List<Role>>.Success(new List<Role> { new Role { Name = "User" } }));

        _commiterMock.Setup(c => c.Users.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.Username.Should().Be(dto.Username);

        _commiterMock.Verify(c => c.Users.AddAsync(It.IsAny<User>()), Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenCommitThrowsException()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Username = "newuser",
            Password = "password123",
            RoleName = "User"
        };
        var command = new RegisterUserCommand(dto);

        _validatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _commiterMock
            .Setup(c => c.Roles.GetAllAsync(
                  It.IsAny<Expression<Func<Role, bool>>?>(),
                  It.IsAny<Func<IQueryable<Role>, IIncludableQueryable<Role, object>>?>(),
                  It.IsAny<Func<IQueryable<Role>, IOrderedQueryable<Role>>?>()
                ))
            .ReturnsAsync(
               DbRequest<List<Role>>.Success(new List<Role> { new Role { Name = "User" } })
            );

        _commiterMock
            .Setup(c => c.Users.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock
            .Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Commit failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("An error occurred while registering the user.");
    }

}