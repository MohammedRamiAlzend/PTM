using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.Commands;
using PTM.Application.DTOs.ProjectDTOs;
using PTM.Domain.CommunicationModels;
using PTM.Domain.Entities;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Net;
using Xunit;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IEntityCommiter> _commiterMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateProjectDto>> _validatorMock;
    private readonly Mock<ILogger<CreateProjectCommand>> _loggerMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _commiterMock = new Mock<IEntityCommiter>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateProjectDto>>();
        _loggerMock = new Mock<ILogger<CreateProjectCommand>>();

        _handler = new CreateProjectCommandHandler(
            _commiterMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailure_WhenValidationFails()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "", Description = "" };
        var command = new CreateProjectCommand(dto);

        _validatorMock.Setup(v => v.Validate(dto)).Returns(
            new ValidationResult(new List<ValidationFailure> {
                new("Name", "Name is required")
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.NotAcceptable);
        result.Message.Should().Contain("Name is required");

        _commiterMock.Verify(c => c.Projects.AddAsync(It.IsAny<Project>()), Times.Never);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAddProjectFails()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "New Project", Description = "Some Desc" };
        var command = new CreateProjectCommand(dto);

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());

        _commiterMock.Setup(c => c.Projects.AddAsync(It.IsAny<Project>()))
            .ReturnsAsync(DbRequest.Failure("DB error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(HttpStatusCode.InternalServerError);
        result.Message.Should().Contain("DB error");

        _commiterMock.Verify(c => c.Projects.AddAsync(It.IsAny<Project>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProjectIsCreatedSuccessfully()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "Valid Project", Description = "Details" };
        var command = new CreateProjectCommand(dto);

        var createdProject = new Project { Id = 1, Name = dto.Name, Description = dto.Description };

        var responseDto = new ProjectResponseDto
        {
            Id = createdProject.Id,
            Name = createdProject.Name,
            Description = createdProject.Description,
            Tasks = []
        };

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());

        _commiterMock.Setup(c => c.Projects.AddAsync(It.IsAny<Project>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.Commit())
            .Returns(1);

        _mapperMock.Setup(m => m.Map<ProjectResponseDto>(It.IsAny<Project>()))
            .Returns(responseDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(dto.Name);

        _commiterMock.Verify(c => c.Projects.AddAsync(It.IsAny<Project>()), Times.Once);
        _commiterMock.Verify(c => c.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCommitThrowsException()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "Throw Project", Description = "Bad DB" };
        var command = new CreateProjectCommand(dto);

        _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());

        _commiterMock.Setup(c => c.Projects.AddAsync(It.IsAny<Project>()))
            .ReturnsAsync(DbRequest.Success());

        _commiterMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Commit failed"));

        _mapperMock.Setup(m => m.Map<ProjectResponseDto>(It.IsAny<Project>()))
            .Returns(new ProjectResponseDto());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
