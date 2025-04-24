using FluentAssertions;
using Moq;
using Peo.ContentManagement.Application.UseCases.Course.Create;
using Peo.Core.Interfaces.Data;

namespace Peo.Tests.UnitTests.ContentManagement.Course;

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
    public async Task Handle_ShouldCreateCourse_WhenValid()
    {
        // Arrange
        var command = new Command(
            Title: "Test Course",
            Description: "Test Description",
            InstructorId: Guid.NewGuid(),
            ProgramContent: "Test Program Content",
            Price: 99.99m,
            Tags: ["test", "course"]
        );

        _repositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CourseId.Should().NotBeEmpty();

        _repositoryMock.Verify(x => x.Insert(It.Is<Peo.ContentManagement.Domain.Entities.Course>(c =>
            c.Title == command.Title &&
            c.Description == command.Description &&
            c.InstructorId == command.InstructorId &&
            c.ProgramContent.Content == command.ProgramContent &&
            c.Price == command.Price &&
            c.Tags.SequenceEqual(command.Tags!)
        )), Times.Once);

        _repositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}