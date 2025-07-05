using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.Commands;
using PTM.Application.DTOs.AuthDTOs;
using PTM.Domain.CommunicationModels;
using PTM.Domain.Entities;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Net;
using Xunit;
using Microsoft.AspNet.Identity;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Needed for Include

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IEntityCommiter> _commiterMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<LoginUserCommandHandler>> _loggerMock;
    private readonly LoginUserCommandHandler _handler;
    private readonly PasswordHasher _passwordHasher;

    public LoginUserCommandHandlerTests()
    {
        _commiterMock = new Mock<IEntityCommiter>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<LoginUserCommandHandler>>();
        _passwordHasher = new PasswordHasher();

        _configMock.Setup(c => c["JwtSettings:SecretKey"])
                   .Returns("thisisalongandsecurekeyforjwttokengenerationthatshouldbeatapproximately32characters"); // Mock a secret key

        _handler = new LoginUserCommandHandler(
            _commiterMock.Object,
            _configMock.Object,
            _loggerMock.Object);
    }

    //[Fact]
    //public async Task Handle_ShouldReturnUnauthorized_WhenUserNotFound()
    //{
    //    // Arrange
    //    var dto = new LoginRequestDto { UserName = "nonexistent", Password = "password" };
    //    var command = new LoginUserCommand(dto);

    //    _commiterMock.Setup(c => c.Users.GetAsync(
    //        It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
    //        It.IsAny<System.Linq.Expressions.Expression<System.Func<User, object>>>()
    //    ))
    //    .ReturnsAsync(DbRequest<User>.Failure("User not found"));

    //    // Act
    //    var result = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsSuccess.Should().BeFalse();
    //    result.Code.Should().Be(HttpStatusCode.Unauthorized);
    //    result.Message.Should().Contain("User not found");
    //}

    //[Fact]
    //public async Task Handle_ShouldReturnUnauthorized_WhenIncorrectPassword()
    //{
    //    // Arrange
    //    var username = "testuser";
    //    var password = "correctpassword";
    //    var incorrectPassword = "wrongpassword";
    //    var hashedPassword = _passwordHasher.HashPassword(password);

    //    var user = new User { Username = username, PasswordHash = hashedPassword, Role = new Role { Name = "User" } };
    //    var dto = new LoginRequestDto { UserName = username, Password = incorrectPassword };
    //    var command = new LoginUserCommand(dto);

    //    _commiterMock.Setup(c => c.Users.GetAsync(
    //        It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
    //        It.IsAny<System.Linq.Expressions.Expression<System.Func<User, object>>>()
    //    ))
    //    .ReturnsAsync(DbRequest<User>.Success(user));

    //    // Act
    //    var result = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsSuccess.Should().BeFalse();
    //    result.Code.Should().Be(HttpStatusCode.Unauthorized);
    //    result.Message.Should().Contain("User Name or password is wrong");
    //}

    //[Fact]
    //public async Task Handle_ShouldReturnSuccess_WhenLoginSuccessful()
    //{
    //    // Arrange
    //    var username = "testuser";
    //    var password = "correctpassword";
    //    var hashedPassword = _passwordHasher.HashPassword(password);

    //    var user = new User { Username = username, PasswordHash = hashedPassword, Role = new Role { Name = "User" } };
    //    var dto = new LoginRequestDto { UserName = username, Password = password };
    //    var command = new LoginUserCommand(dto);

    //    _commiterMock.Setup(c => c.Users.GetAsync(
    //        It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
    //        It.IsAny<System.Linq.Expressions.Expression<System.Func<User, object>>>()
    //    ))
    //    .ReturnsAsync(DbRequest<User>.Success(user));

    //    // Act
    //    var result = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsSuccess.Should().BeTrue();
    //    result.Code.Should().Be(HttpStatusCode.OK);
    //    result.Data.Should().NotBeNull();
    //    result.Data!.Username.Should().Be(username);
    //    result.Data.Role.Should().Be("User");
    //    result.Data.Token.Should().NotBeNullOrEmpty();
    //}
} 