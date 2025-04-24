using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Peo.Identity.Application.Endpoints;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Identity.Domain.Interfaces.Services;
using Xunit;

namespace Peo.Tests.UnitTests.Identity.Endpoints;

public class LoginEndpointTests
{
    private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginRequest _validRequest;

    public LoginEndpointTests()
    {
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            null, null, null, null, null, null, null, null);

        // Configure UserManager behavior
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => new IdentityUser { Email = email });

        userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new[] { "Student" });

        _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
            null, null, null, null);

        _tokenServiceMock = new Mock<ITokenService>();
        _validRequest = new LoginRequest(
            Email: "test@example.com",
            Password: "Test@123"
        );
    }

    [Fact]
    public async Task HandleLogin_ShouldReturnValidationProblem_WhenRequestIsInvalid()
    {
        // Arrange
        var invalidRequest = new LoginRequest("", "");

        // Act
        var result = await LoginEndpoint.HandleLogin(
            invalidRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }

    [Fact]
    public async Task HandleLogin_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var userManager = _signInManagerMock.Object.UserManager;
        Mock.Get(userManager).Setup(x => x.FindByEmailAsync(_validRequest.Email))
            .ReturnsAsync((IdentityUser?)null);

        // Act
        var result = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task HandleLogin_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new IdentityUser { Id = Guid.NewGuid().ToString() };
        var userManager = _signInManagerMock.Object.UserManager;

        Mock.Get(userManager).Setup(x => x.FindByEmailAsync(_validRequest.Email))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, _validRequest.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task HandleLogin_ShouldReturnOk_WhenLoginSucceeds()
    {
        // Arrange
        var user = new IdentityUser { Id = Guid.NewGuid().ToString() };
        var roles = new[] { "Student" };
        var token = "test-token";

        // Configure UserManager behavior
        var userManager = _signInManagerMock.Object.UserManager;
        Mock.Get(userManager).Setup(x => x.FindByEmailAsync(_validRequest.Email))
            .ReturnsAsync(user);

        Mock.Get(userManager).Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, _validRequest.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _tokenServiceMock.Setup(x => x.CreateToken(user, roles))
            .Returns(token);

        // Act
        var result = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        result.Should().BeOfType<Ok<LoginResponse>>();

        var response = ((Ok<LoginResponse>)result).Value;
        response.Should().NotBeNull();
        response.Token.Should().Be(token);
        response.UserId.Should().Be(Guid.Parse(user.Id));
    }
} 