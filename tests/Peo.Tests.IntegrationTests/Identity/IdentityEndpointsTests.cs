using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Peo.Core.Dtos;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Xunit;

namespace Peo.Tests.IntegrationTests.Identity;

public class IdentityEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IServiceScope _scope;

    public IdentityEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    }

    public async Task InitializeAsync()
    {
        //// Clean up any existing test data
        //var users = _userManager.Users.ToList();
        //foreach (var user in users)
        //{
        //    await _userManager.DeleteAsync(user);
        //}
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldCreateUser()
    {
        // Arrange
        var request = new RegisterRequest(
            Email: $"{Guid.NewGuid()}@example.com",
            Password: "Test123!",
            Name: "Test User"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var user = await _userManager.FindByEmailAsync(request.Email);
        user.Should().NotBeNull();
        user!.Email.Should().Be(request.Email);
        user.UserName.Should().Be(request.Email);
        
        var isInRole = await _userManager.IsInRoleAsync(user, "Student");
        isInRole.Should().BeTrue();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnValidationError()
    {
        // Arrange
        var request = new RegisterRequest(
            Email: "invalid-email",
            Password: "Test123!",
            Name: "Test User"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        var password = "Test123!";
        
        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };
        
        await _userManager.CreateAsync(user, password);
        await _userManager.AddToRoleAsync(user, "Student");

        var request = new LoginRequest(email, password);

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(Guid.Parse(user.Id));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "WrongPassword");

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    //[Fact]
    //public async Task RefreshToken_WithValidToken_ShouldReturnNewToken()
    //{
    //    // Arrange
    //    var email = "test@example.com";
    //    var password = "Test123!";
        
    //    var user = new IdentityUser
    //    {
    //        UserName = email,
    //        Email = email,
    //        EmailConfirmed = true
    //    };
        
    //    await _userManager.CreateAsync(user, password);
    //    await _userManager.AddToRoleAsync(user, "Student");

    //    // Get initial token
    //    var loginRequest = new LoginRequest(email, password);
    //    var loginResponse = await _client.PostAsJsonAsync("/v1/identity/login", loginRequest);
    //    var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

    //    // Wait for token to expire
    //    await Task.Delay(TimeSpan.FromMinutes(1));

    //    var refreshRequest = new RefreshTokenRequest(loginResult!.Token);

    //    // Act
    //    var response = await _client.PostAsJsonAsync("/v1/identity/refresh-token", refreshRequest);

    //    // Assert
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
    //    result.Should().NotBeNull();
    //    result!.Token.Should().NotBeNullOrEmpty();
    //    result.Token.Should().NotBe(loginResult.Token);
    //    result.UserId.Should().Be(Guid.Parse(user.Id));
    //}

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new RefreshTokenRequest("invalid-token");

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/refresh-token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
} 