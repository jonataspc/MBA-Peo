using FluentAssertions;
using Mapster;
using MediatR;
using Moq;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Application.UseCases.Course.GetById;
using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using Xunit;

namespace Peo.Tests.UnitTests.ContentManagement.Course;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Peo.ContentManagement.Domain.Entities.Course>> _repositoryMock;
    private readonly Handler _handler;

    public GetByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Peo.ContentManagement.Domain.Entities.Course>>();
        _handler = new Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCourse_WhenFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Peo.ContentManagement.Domain.Entities.Course(
            title: "Test Course",
            description: "Test Description",
            instructorId: Guid.NewGuid(),
            programContent: new ProgramContent("Test Program Content"),
            price: 99.99m,
            isPublished: true,
            publishedAt: DateTime.Now,
            tags: ["test", "course"],
            lessons: []
        );

        _repositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync(course);

        var query = new Query(courseId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Course.Should().NotBeNull();
        result.Value.Course.Should().BeEquivalentTo(course.Adapt<CourseResponse>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNullCourse_WhenNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync((Peo.ContentManagement.Domain.Entities.Course?)null);

        var query = new Query(courseId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Course.Should().BeNull();
    }
} 