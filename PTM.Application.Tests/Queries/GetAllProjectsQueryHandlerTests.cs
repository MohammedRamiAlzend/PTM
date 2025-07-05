using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PTM.Application.Queries;
using PTM.Application.DTOs.ProjectDTOs;
using PTM.Domain.CommunicationModels;
using PTM.Domain.Entities;
using PTM.Infrastructure.Repositories.Interfaces;
using System.Net;
using Xunit;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using PTM.Application.DTOs.TaskDTOs;

namespace PTM.Application.Tests.Queries
{
    public class GetAllProjectsQueryHandlerTests
    {
        private readonly Mock<IEntityCommiter> _commiterMock;
        private readonly Mock<ILogger<GetProjectsQuery>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetProjectsQueryHandler _handler;

        public GetAllProjectsQueryHandlerTests()
        {
            _commiterMock = new Mock<IEntityCommiter>();
            _loggerMock = new Mock<ILogger<GetProjectsQuery>>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetProjectsQueryHandler(
                _commiterMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenProjectsAreRetrievedSuccessfully()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project A", Tasks = new List<AppTask>() },
                new Project { Id = 2, Name = "Project B", Tasks = new List<AppTask>() }
            };
            var projectResponseDtos = new List<ProjectResponseDto>
            {
                new ProjectResponseDto { Id = 1, Name = "Project A", Tasks = new List<TaskResponseDto>() },
                new ProjectResponseDto { Id = 2, Name = "Project B", Tasks = new List<TaskResponseDto>() }
            };

            _commiterMock.Setup(c => c.Projects.GetAllAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>(),
                It.IsAny<Func<IQueryable<Project>, IOrderedQueryable<Project>>>()
            ))
            .ReturnsAsync(DbRequest<List<Project>>.Success(projects));

            _mapperMock.Setup(m => m.Map<List<ProjectResponseDto>>(It.IsAny<List<Project>>()))
                .Returns(projectResponseDtos);

            var query = new GetProjectsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            _loggerMock.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenProjectRetrievalFails()
        {
            // Arrange
            _commiterMock.Setup(c => c.Projects.GetAllAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>(),
                It.IsAny<Func<IQueryable<Project>, IOrderedQueryable<Project>>>()
            ))
            .ReturnsAsync(DbRequest<List<Project>>.Failure("Database error"));

            var query = new GetProjectsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Contain("Database error");
            _loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
} 