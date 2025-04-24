using FluentAssertions;
using Moq;
using Peo.ContentManagement.Application.Services;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Interfaces.Data;
using Xunit;

namespace Peo.Tests.UnitTests.ContentManagement;

public class CourseLessonServiceTests
{
    private readonly Mock<IRepository<Course>> _courseRepositoryMock;
    private readonly CourseLessonService _courseLessonService;

    public CourseLessonServiceTests()
    {
        _courseRepositoryMock = new Mock<IRepository<Course>>();
        _courseLessonService = new CourseLessonService(_courseRepositoryMock.Object);
    }

    [Fact]
    public async Task CheckIfCourseExistsAsync_ShouldReturnTrueWhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", Guid.NewGuid(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Lesson>());
        
        _courseRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _courseLessonService.CheckIfCourseExistsAsync(courseId);

        // Assert
        result.Should().BeTrue();
        _courseRepositoryMock.Verify(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task CheckIfCourseExistsAsync_ShouldReturnFalseWhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        
        _courseRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>()))
            .ReturnsAsync(false);

        // Act
        var result = await _courseLessonService.CheckIfCourseExistsAsync(courseId);

        // Assert
        result.Should().BeFalse();
        _courseRepositoryMock.Verify(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCoursePriceAsync_ShouldReturnCoursePrice()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedPrice = 99.99m;
        var course = new Course("Test Course", "Description", Guid.NewGuid(), null, expectedPrice, true, DateTime.UtcNow, new List<string>(), new List<Lesson>());
        
        _courseRepositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseLessonService.GetCoursePriceAsync(courseId);

        // Assert
        result.Should().Be(expectedPrice);
        _courseRepositoryMock.Verify(x => x.GetAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task GetCourseTitleAsync_ShouldReturnCourseTitle()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedTitle = "Test Course";
        var course = new Course(expectedTitle, "Description", Guid.NewGuid(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Lesson>());
        
        _courseRepositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseLessonService.GetCourseTitleAsync(courseId);

        // Assert
        result.Should().Be(expectedTitle);
        _courseRepositoryMock.Verify(x => x.GetAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task GetTotalLessonsInCourseAsync_ShouldReturnLessonCount()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedCount = 10;
        var lessons = Enumerable.Range(0, expectedCount)
            .Select(_ => new Lesson("Test Lesson", "Description", "video-url", TimeSpan.FromMinutes(30), new List<LessonFile>(), courseId))
            .ToList();
        var course = new Course("Test Course", "Description", Guid.NewGuid(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), lessons);
        
        _courseRepositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseLessonService.GetTotalLessonsInCourseAsync(courseId);

        // Assert
        result.Should().Be(expectedCount);
        _courseRepositoryMock.Verify(x => x.GetAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task GetTotalLessonsInCourseAsync_ShouldReturnZeroWhenNoLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", Guid.NewGuid(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Lesson>());
        
        _courseRepositoryMock.Setup(x => x.GetAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseLessonService.GetTotalLessonsInCourseAsync(courseId);

        // Assert
        result.Should().Be(0);
        _courseRepositoryMock.Verify(x => x.GetAsync(courseId), Times.Once);
    }
}