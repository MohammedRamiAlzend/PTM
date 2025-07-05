using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Application.Queries;
using PTM.Domain.CommunicationModels;
using PTM.Domain.Entities;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PTM.Application.Tests.Queries
{
    public class GetAllTasksQueryHandlerTests
    {
        private readonly Mock<IEntityCommiter> _commiterMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<GetTasksQuery>> _loggerMock;
        private readonly GetTasksQueryHandler _handler;

        public GetAllTasksQueryHandlerTests()
        {
            _commiterMock = new Mock<IEntityCommiter>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<GetTasksQuery>>();

            _handler = new GetTasksQueryHandler(
                _commiterMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenProjectNotFound()
        {
            // Arrange
            var projectId = 99;
            var query = new GetTasksQuery(projectId);

            _commiterMock.Setup(c => c.Projects.AnyAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()
            ))
            .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Contain($"project with {projectId} was not found");
            _commiterMock.Verify(c => c.Tasks.GetAllAsync(
                It.IsAny<Expression<Func<AppTask, bool>>?>(),
                It.IsAny<Func<IQueryable<AppTask>, IIncludableQueryable<AppTask, object>>>(),
                It.IsAny<Func<IQueryable<AppTask>, IOrderedQueryable<AppTask>>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTaskRetrievalFails()
        {
            // Arrange
            var projectId = 1;
            var query = new GetTasksQuery(projectId);

            _commiterMock.Setup(c => c.Projects.AnyAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()
            ))
            .ReturnsAsync(true);

            _commiterMock.Setup(c => c.Tasks.GetAllAsync(
                It.IsAny<Expression<Func<AppTask, bool>>>(),
                It.IsAny<Func<IQueryable<AppTask>, IIncludableQueryable<AppTask, object>>>(),
                It.IsAny<Func<IQueryable<AppTask>, IOrderedQueryable<AppTask>>>()
            ))
            .ReturnsAsync(DbRequest<List<AppTask>>.Failure("Database error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Contain("Database error");
            _loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTasksAreRetrievedSuccessfully()
        {
            // Arrange
            var projectId = 1;
            var query = new GetTasksQuery(projectId);
            var tasks = new List<AppTask>
            {
                new AppTask { Id = 1, Title = "Task 1", ProjectId = projectId },
                new AppTask { Id = 2, Title = "Task 2", ProjectId = projectId }
            };
            var taskResponseDtos = new List<TaskResponseDto>
            {
                new TaskResponseDto { Id = 1, Title = "Task 1" },
                new TaskResponseDto { Id = 2, Title = "Task 2" }
            };

            _commiterMock.Setup(c => c.Projects.AnyAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()
            ))
            .ReturnsAsync(true);

            _commiterMock.Setup(c => c.Tasks.GetAllAsync(
                It.IsAny<Expression<Func<AppTask, bool>>>(),
                It.IsAny<Func<IQueryable<AppTask>, IIncludableQueryable<AppTask, object>>>(),
                It.IsAny<Func<IQueryable<AppTask>, IOrderedQueryable<AppTask>>>()
            ))
            .ReturnsAsync(DbRequest<List<AppTask>>.Success(tasks));

            _mapperMock.Setup(m => m.Map<List<TaskResponseDto>>(It.IsAny<List<AppTask>>()))
                .Returns(taskResponseDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            _loggerMock.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Exactly(2));
        }
    }
}