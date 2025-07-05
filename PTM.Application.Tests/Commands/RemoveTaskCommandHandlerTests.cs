using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.Commands;
using PTM.Domain.CommunicationModels;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Net;
using Xunit;

public class RemoveTaskCommandHandlerTests
{
    private readonly Mock<IEntityCommiter> _commiterMock;
    private readonly Mock<ILogger<RemoveTaskCommandHandle>> _loggerMock;
    private readonly RemoveTaskCommandHandle _handler;

    public RemoveTaskCommandHandlerTests()
    {
        _commiterMock = new Mock<IEntityCommiter>();
        _loggerMock = new Mock<ILogger<RemoveTaskCommandHandle>>();

        _handler = new RemoveTaskCommandHandle(
            _commiterMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRemoveTaskFails()
    {
        // Arrange
        var taskId = 1;
        var command = new RemoveTaskCommand(taskId);

        _commiterMock.Setup(c => c.Tasks.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.AppTask, bool>>>()))
            .ReturnsAsync(DbRequest.Failure("Failed to remove task from DB"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("Failed to remove task from DB");

        _commiterMock.Verify(c => c.Tasks.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.AppTask, bool>>>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var taskId = 1;
        var command = new RemoveTaskCommand(taskId);

        _commiterMock.Setup(c => c.Tasks.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.AppTask, bool>>>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);

        _commiterMock.Verify(c => c.Tasks.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.AppTask, bool>>>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTaskRemovedSuccessfully()
    {
        // Arrange
        var taskId = 1;
        var command = new RemoveTaskCommand(taskId);

        _commiterMock.Setup(c => c.Tasks.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.AppTask, bool>>>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(HttpStatusCode.OK);

        _commiterMock.Verify(c => c.Tasks.RemoveAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PTM.Domain.Entities.AppTask, bool>>>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
} 