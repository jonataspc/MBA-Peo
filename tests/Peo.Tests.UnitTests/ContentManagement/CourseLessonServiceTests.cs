using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Peo.ContentManagement.Application.Services;
using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Infra.Data.Contexts;
using Xunit;

namespace Peo.Tests.UnitTests.ContentManagement;

public class CourseLessonServiceTests
{
    private readonly Mock<ContentManagementContext> _contextMock;
    private readonly CourseLessonService _courseLessonService;

    public CourseLessonServiceTests()
    {
        _contextMock = new Mock<ContentManagementContext>();
        _courseLessonService = new CourseLessonService(_contextMock.Object);
    }

    [Fact]
    public async Task CheckIfCourseExistsAsync_ShouldReturnTrueWhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", Guid.NewGuid(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Lesson>());
        var mockSet = new Mock<DbSet<Course>>();
        
        mockSet.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _contextMock.Setup(x => x.Courses)
            .Returns(mockSet.Object);

        // Act
        var result = await _courseLessonService.CheckIfCourseExistsAsync(courseId);

        // Assert
        result.Should().BeTrue();
        mockSet.Verify(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckIfCourseExistsAsync_ShouldReturnFalseWhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var mockSet = new Mock<DbSet<Course>>();
        
        mockSet.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _contextMock.Setup(x => x.Courses)
            .Returns(mockSet.Object);

        // Act
        var result = await _courseLessonService.CheckIfCourseExistsAsync(courseId);

        // Assert
        result.Should().BeFalse();
        mockSet.Verify(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCoursePriceAsync_ShouldReturnCoursePrice()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedPrice = 99.99m;
        var course = new Course("Test Course", "Description", Guid.NewGuid(), null, expectedPrice, true, DateTime.UtcNow, new List<string>(), new List<Lesson>());
        var mockSet = new Mock<DbSet<Course>>();
        
        mockSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        
        _contextMock.Setup(x => x.Courses)
            .Returns(mockSet.Object);

        // Act
        var result = await _courseLessonService.GetCoursePriceAsync(courseId);

        // Assert
        result.Should().Be(expectedPrice);
        mockSet.Verify(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCourseTitleAsync_ShouldReturnCourseTitle()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedTitle = "Test Course";
        var course = new Course(expectedTitle, "Description", Guid.NewGuid(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Lesson>());
        var mockSet = new Mock<DbSet<Course>>();
        
        mockSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        
        _contextMock.Setup(x => x.Courses)
            .Returns(mockSet.Object);

        // Act
        var result = await _courseLessonService.GetCourseTitleAsync(courseId);

        // Assert
        result.Should().Be(expectedTitle);
        mockSet.Verify(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTotalLessonsInCourseAsync_ShouldReturnLessonCount()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedCount = 10;
        var mockSet = new Mock<DbSet<Lesson>>();
        
        mockSet.Setup(x => x.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);
        
        _contextMock.Setup(x => x.Lessons)
            .Returns(mockSet.Object);

        // Act
        var result = await _courseLessonService.GetTotalLessonsInCourseAsync(courseId);

        // Assert
        result.Should().Be(expectedCount);
        mockSet.Verify(x => x.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTotalLessonsInCourseAsync_ShouldReturnZeroWhenNoLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var mockSet = new Mock<DbSet<Lesson>>();
        
        mockSet.Setup(x => x.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        
        _contextMock.Setup(x => x.Lessons)
            .Returns(mockSet.Object);

        // Act
        var result = await _courseLessonService.GetTotalLessonsInCourseAsync(courseId);

        // Assert
        result.Should().Be(0);
        mockSet.Verify(x => x.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Lesson, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
} 