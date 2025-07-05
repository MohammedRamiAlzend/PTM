using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.Commands;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.CommunicationModels;
using PTM.Domain.Entities;
using PTM.Domain.Entities.Enums;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;
using Xunit;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<IEntityCommiter> _commiterMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateTaskDto>> _validatorMock;
    private readonly Mock<ILogger<CreateTaskCommand>> _loggerMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _commiterMock = new Mock<IEntityCommiter>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateTaskDto>>();
        _loggerMock = new Mock<ILogger<CreateTaskCommand>>();

        _handler = new CreateTaskCommandHandler(
            _commiterMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailure_WhenValidationFails()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "", DueDate = DateTime.Now.AddDays(-1) };
        var command = new CreateTaskCommand(1, dto);

        _validatorMock.Setup(v => v.Validate(dto)).Returns(
            new ValidationResult(new List<ValidationFailure> {
                new("Title", "Title is required"),
                new("DueDate", "DueDate must be in the future")
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.NotAcceptable);
        result.Message.Should().Contain("Title is required");
        result.Message.Should().Contain("DueDate must be in the future");

        _commiterMock.Verify(c => c.Tasks.AddAsync(It.IsAny<AppTask>()), Times.Never);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProjectNotFound()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "New Task", DueDate = DateTime.Now.AddDays(1) };
        var command = new CreateTaskCommand(99, dto); // Non-existent project ID

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _commiterMock.Setup(c => c.Projects.AnyAsync(
            It.IsAny<Expression<Func<Project, bool>>>(),
            It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>?>()))
                     .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.NotFound);
        result.Message.Should().Contain("project with 99 was not found");

        _commiterMock.Verify(c => c.Tasks.AddAsync(It.IsAny<AppTask>()), Times.Never);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAddTaskFails()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "New Task", DueDate = DateTime.Now.AddDays(1) };
        var command = new CreateTaskCommand(1, dto);

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _commiterMock.Setup(c => c.Projects.AnyAsync(
            It.IsAny<Expression<Func<Project, bool>>>(),
            It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>?>())
        ).ReturnsAsync(true);
        _commiterMock.Setup(c => c.Tasks.AddAsync(It.IsAny<AppTask>()))
            .ReturnsAsync(DbRequest.Failure("DB error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("DB error");

        _commiterMock.Verify(c => c.Tasks.AddAsync(It.IsAny<AppTask>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTaskIsCreatedSuccessfully()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "Valid Task", DueDate = DateTime.Now.AddDays(1) };
        var command = new CreateTaskCommand(1, dto);

        var createdTask = new AppTask { Id = 1, Title = dto.Title, DueDate = dto.DueDate, ProjectId = 1 };

        var responseDto = new TaskResponseDto
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            DueDate = createdTask.DueDate,
            Status = AppTaskStatus.Pending.ToString(),
        };

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _commiterMock.Setup(c => c.Projects.AnyAsync(
                    It.IsAny<Expression<Func<Project, bool>>>(),
                    It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>?>()
                    ))
        .ReturnsAsync(true);
        _commiterMock.Setup(c => c.Tasks.AddAsync(It.IsAny<AppTask>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TaskResponseDto>(It.IsAny<AppTask>()))
            .Returns(responseDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(dto.Title);

        _commiterMock.Verify(c => c.Tasks.AddAsync(It.IsAny<AppTask>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCommitThrowsException()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "Throw Task", DueDate = DateTime.Now.AddDays(1) };
        var command = new CreateTaskCommand(1, dto);

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());
        _commiterMock.Setup(c => c.Projects.AnyAsync(
            It.IsAny<Expression<Func<Project, bool>>>(),
            It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>?>()))
                     .ReturnsAsync(true);
        _commiterMock.Setup(c => c.Tasks.AddAsync(It.IsAny<AppTask>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Commit failed"));

        _mapperMock.Setup(m => m.Map<TaskResponseDto>(It.IsAny<AppTask>()))
            .Returns(new TaskResponseDto());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
} 