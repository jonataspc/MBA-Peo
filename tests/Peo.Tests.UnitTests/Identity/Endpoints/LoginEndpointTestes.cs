using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Peo.Core.Dtos;
using Peo.Identity.Application.Endpoints;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Tests.UnitTests.Identity.Endpoints;

public class LoginEndpointTests
{
    private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginRequest _validRequest;
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly JwtSettings _jwtSettings;

    public LoginEndpointTests()
    {
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();

        // Setup JwtSettings
        _jwtSettings = new JwtSettings
        {
            Key = "7c02bcde06b6b2c8711775145b694135a314503b9111edc4434d4312c2a6b49beee4f7e6e518f6a275cc18affc189427245dbac648378020fb97e9ec4a8559e9a156f7bbfa0d0e4bc8287960f1e77b671713472f7c67a0e922a6cf275ee64e451d3c788233e9066ef54dce462446ec39ed90a4c0d2c4aa25ba79f1375322ada48bdff86f693f83c250f423be4f96c04b505d8822f559408b6da1544c4c63c3e15ff325c641f5a9f7591200b39241d55f47757673a25e0a0721fdc74defdea9091b42555ecef984c58059d296d88afc5c356b5d7178bad9c166745a16218cddf028553e7e3e042e499cba33dbee934aa5bfee5fc86d23bb0cde3c23ed10c52c7f",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };
        _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);

        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        // Configure UserManager behavior
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => new IdentityUser { Email = email });

        userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new[] { "Student" });

        _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
            null!, null!, null!, null!);

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
        var user = new IdentityUser { Id = Guid.CreateVersion7().ToString() };
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
        var user = new IdentityUser { Id = Guid.CreateVersion7().ToString() };
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
        response!.Token.Should().Be(token);
        response!.UserId.Should().Be(Guid.Parse(user.Id));
    }

    [Fact]
    public async Task HandleRefreshToken_ShouldReturnFail_WhenTokenIsStillValid()
    {
        // Arrange
        var user = new IdentityUser { Id = Guid.CreateVersion7().ToString() };
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

        var resultToken = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Act
        var response = ((Ok<LoginResponse>)resultToken).Value;

        var result = await RefreshTokenEndpoint.HandleRefreshToken(new RefreshTokenRequest(response!.Token), userManager, _jwtSettingsMock.Object, _tokenServiceMock.Object, new Mock<ILogger<RefreshTokenEndpoint>>().Object);

        // Assert
        result.Should().BeOfType<UnauthorizedHttpResult>();
    }
}