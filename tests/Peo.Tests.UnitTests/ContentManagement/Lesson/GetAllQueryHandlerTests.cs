using FluentAssertions;
using Mapster;
using MediatR;
using Moq;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Application.UseCases.Lesson.GetAll;
using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using Xunit;

namespace Peo.Tests.UnitTests.ContentManagement.Lesson;

public class GetAllQueryHandlerTests
{
    private readonly Mock<IRepository<Peo.ContentManagement.Domain.Entities.Course>> _repositoryMock;
    private readonly Handler _handler;

    public GetAllQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Peo.ContentManagement.Domain.Entities.Course>>();
        _handler = new Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllLessons_WhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lessons = new List<Peo.ContentManagement.Domain.Entities.Lesson>
        {
            new(
                title: "Test Lesson 1",
                description: "Test Lesson Description 1",
                videoUrl: "https://example.com/video1",
                duration: TimeSpan.FromMinutes(30),
                files: [],
                courseId: courseId
            ),
            new(
                title: "Test Lesson 2",
                description: "Test Lesson Description 2",
                videoUrl: "https://example.com/video2",
                duration: TimeSpan.FromMinutes(45),
                files: [],
                courseId: courseId
            )
        };

        var course = new Peo.ContentManagement.Domain.Entities.Course(
            title: "Test Course",
            description: "Test Description",
            instructorId: Guid.NewGuid(),
            programContent: new ProgramContent("Test Program Content"),
            price: 99.99m,
            isPublished: true,
            publishedAt: DateTime.Now,
            tags: ["test", "course"],
            lessons: lessons
        );

        _repositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync(course);

        var query = new Query(courseId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Lessons.Should().NotBeNull();
        result.Value.Lessons.Should().HaveCount(2);
        result.Value.Lessons.Should().BeEquivalentTo(lessons.Adapt<IEnumerable<LessonResponse>>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCourseNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync((Peo.ContentManagement.Domain.Entities.Course?)null);

        var query = new Query(courseId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Course does not exist");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenCourseHasNoLessons()
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
        result.Value.Lessons.Should().NotBeNull();
        result.Value.Lessons.Should().BeEmpty();
    }
} 