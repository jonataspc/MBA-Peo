using FluentAssertions;
using Mapster;
using MediatR;
using Moq;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Application.UseCases.Course.GetAll;
using Peo.ContentManagement.Domain.Entities;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;
using Xunit;

namespace Peo.Tests.UnitTests.ContentManagement.Course;

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
    public async Task Handle_ShouldReturnAllCourses()
    {
        // Arrange
        var courses = new List<Peo.ContentManagement.Domain.Entities.Course>
        {
            new(
                title: "Test Course 1",
                description: "Test Description 1",
                instructorId: Guid.CreateVersion7(),
                programContent: new ProgramContent("Test Program Content 1"),
                price: 99.99m,
                isPublished: true,
                publishedAt: DateTime.Now,
                tags: ["test", "course"],
                lessons: []
            ),
            new(
                title: "Test Course 2",
                description: "Test Description 2",
                instructorId: Guid.CreateVersion7(),
                programContent: new ProgramContent("Test Program Content 2"),
                price: 199.99m,
                isPublished: true,
                publishedAt: DateTime.Now,
                tags: ["test", "course"],
                lessons: []
            )
        };

        _repositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(courses);

        var query = new Query();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Courses.Should().NotBeNull();
        result.Value.Courses.Should().HaveCount(2);
        result.Value.Courses.Should().BeEquivalentTo(courses.Adapt<IEnumerable<CourseResponse>>());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCoursesExist()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<Peo.ContentManagement.Domain.Entities.Course>());

        var query = new Query();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Courses.Should().NotBeNull();
        result.Value.Courses.Should().BeEmpty();
    }
} 