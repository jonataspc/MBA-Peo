using FluentAssertions;
using Moq;
using Peo.Core.Entities;
using Peo.Identity.Application.Services;
using Peo.Identity.Domain.Interfaces.Data;
using Xunit;

namespace Peo.Tests.UnitTests.Identity;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserAndCommitChanges()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "Test User", "test@example.com");
        _userRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _userService.AddAsync(user);

        // Assert
        _userRepositoryMock.Verify(x => x.Insert(user), Times.Once);
        _userRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUserWhenFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User(userId, "Test User", "test@example.com");
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNullWhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }
} 