using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Peo.Identity.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Peo.Tests.UnitTests.Identity;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x.GetValue<string>("Jwt:Issuer")).Returns("test-issuer");
        _configurationMock.Setup(x => x.GetValue<string>("Jwt:Audience")).Returns("test-audience");
        _configurationMock.Setup(x => x.GetValue<string>("Jwt:Key")).Returns("test-key-1234567890123456");
        _configurationMock.Setup(x => x["Authentication:ExpirationInMinutes"]).Returns("60");

        _tokenService = new TokenService(_configurationMock.Object);
    }

    [Fact]
    public void CreateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var user = new Microsoft.AspNetCore.Identity.IdentityUser
        {
            Id = "123",
            UserName = "test@example.com"
        };
        var roles = new[] { "Student" };

        // Act
        var token = _tokenService.CreateToken(user, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("test-key-1234567890123456");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "test-issuer",
            ValidateAudience = true,
            ValidAudience = "test-audience",
            ValidateLifetime = true
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        principal.Should().NotBeNull();
        validatedToken.Should().NotBeNull();

        var jwtToken = (JwtSecurityToken)validatedToken;
        jwtToken.Issuer.Should().Be("test-issuer");
        jwtToken.Audiences.Should().Contain("test-audience");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "test@example.com");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Student");
    }
} 