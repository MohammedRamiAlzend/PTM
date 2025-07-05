using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.Commands;
using PTM.Domain.CommunicationModels;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Net;
using Xunit;

public class RemoveProjectCommandHandlerTests
{
    private readonly Mock<IEntityCommiter> _commiterMock;
    private readonly Mock<ILogger<RemoveProjectCommandHandler>> _loggerMock;
    private readonly RemoveProjectCommandHandler _handler;

    public RemoveProjectCommandHandlerTests()
    {
        _commiterMock = new Mock<IEntityCommiter>();
        _loggerMock = new Mock<ILogger<RemoveProjectCommandHandler>>();

        _handler = new RemoveProjectCommandHandler(
            _commiterMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRemoveProjectFails()
    {
        // Arrange
        var projectId = 1;
        var command = new RemoveProjectCommand(projectId);

        _commiterMock.Setup(c => c.Projects.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.Project, bool>>>()))
            .ReturnsAsync(DbRequest.Failure("Failed to remove project from DB"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("Failed to remove project from DB");

        _commiterMock.Verify(c => c.Projects.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.Project, bool>>>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var projectId = 1;
        var command = new RemoveProjectCommand(projectId);

        _commiterMock.Setup(c => c.Projects.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.Project, bool>>>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("Failed to save changes after removing the project.");

        _commiterMock.Verify(c => c.Projects.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.Project, bool>>>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProjectRemovedSuccessfully()
    {
        // Arrange
        var projectId = 1;
        var command = new RemoveProjectCommand(projectId);

        _commiterMock.Setup(c => c.Projects.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.Project, bool>>>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(HttpStatusCode.OK);
        result.Message.Should().Contain("Project removed successfully.");

        _commiterMock.Verify(c => c.Projects.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.Project, bool>>>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
} 