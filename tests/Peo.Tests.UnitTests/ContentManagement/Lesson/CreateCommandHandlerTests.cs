using FluentAssertions;
using Moq;
using Peo.ContentManagement.Application.UseCases.Lesson.Create;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.Interfaces.Data;

namespace Peo.Tests.UnitTests.ContentManagement.Lesson;

public class CreateCommandHandlerTests
{
    private readonly Mock<IRepository<Peo.ContentManagement.Domain.Entities.Course>> _repositoryMock;
    private readonly Handler _handler;

    public CreateCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Peo.ContentManagement.Domain.Entities.Course>>();
        _handler = new Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateLesson_WhenValid()
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

        var command = new Command
        {
            CourseId = courseId,
            Title = "Test Lesson",
            Description = "Test Lesson Description",
            VideoUrl = "https://example.com/video",
            Duration = TimeSpan.FromMinutes(30),
            Files = []
        };

        _repositoryMock.Setup(x => x.WithTracking().GetAsync(courseId))
            .ReturnsAsync(course);
        _repositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.LessonId.Should().NotBeEmpty();

        _repositoryMock.Verify(x => x.WithTracking().GetAsync(courseId), Times.Once);
        _repositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCourseNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var command = new Command
        {
            CourseId = courseId,
            Title = "Test Lesson",
            Description = "Test Lesson Description",
            VideoUrl = "https://example.com/video",
            Duration = TimeSpan.FromMinutes(30),
            Files = []
        };

        _repositoryMock.Setup(x => x.WithTracking().GetAsync(courseId))
            .ReturnsAsync((Peo.ContentManagement.Domain.Entities.Course?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Course not found");

        _repositoryMock.Verify(x => x.WithTracking().GetAsync(courseId), Times.Once);
        _repositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}