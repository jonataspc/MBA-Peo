using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using CourseCreate = Peo.ContentManagement.Application.UseCases.Course.Create;
using CourseGetAll = Peo.ContentManagement.Application.UseCases.Course.GetAll;
using CourseGetById = Peo.ContentManagement.Application.UseCases.Course.GetById;
using LessonCreate = Peo.ContentManagement.Application.UseCases.Lesson.Create;
using LessonGetAll = Peo.ContentManagement.Application.UseCases.Lesson.GetAll;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.DomainObjects.Result;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Tests.IntegrationTests.Setup;
using Xunit;
using Microsoft.AspNetCore.Http;
using Peo.ContentManagement.Application.Dtos;

namespace Peo.Tests.IntegrationTests.ContentManagement;

public class ContentManagementEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestDatabaseSetup _testDb;
    private Guid _testUserId = Guid.CreateVersion7();
    private Course? _testCourse;

    public ContentManagementEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _testDb = new TestDatabaseSetup(_factory.Services);
    }

    public async Task InitializeAsync()
    {
        await _testDb.CreateAdminUser(_testUserId);

        // Create test course
        _testCourse = await _testDb.CreateTestCourseAsync(instructorId: _testUserId);
        await LoginAsync();
    }

    private async Task LoginAsync()
    {
        // Arrange
        var request = new LoginRequest(_testDb.UserTestEmail, _testDb.UserTestPassword);

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();

        _testUserId = result!.UserId;

        // Set the token in the client
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
    }

    public async Task DisposeAsync()
    {
        await _testDb.CleanupAsync();
    }

    [Fact]
    public async Task CreateCourse_WithValidRequest_ShouldCreateCourse()
    {
        // Arrange
        var command = new CourseCreate.Command(
            "Test Course",
            "Test Description",
            _testUserId,
            null,
            99.99m,
            new List<string> { "test", "integration" }
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/content/course/", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CourseCreate.Response>();
        result.Should().NotBeNull();
        result!.CourseId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateCourse_WithInvalidRequest_ShouldReturnValidationError()
    {
        // Arrange
        var command = new CourseCreate.Command(
            "", // Invalid title
            "Test Description",
            _testUserId,
            null,
            99.99m,
            new List<string> { "test", "integration" }
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/content/course/", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        result.Should().NotBeNull();
        result.Errors.Any(e => e.Value.Any(x => x.Contains("The Title field is required."))).Should().BeTrue();
    }

    [Fact]
    public async Task GetCourseById_WithValidId_ShouldReturnCourse()
    {
        // Arrange
        var courseId = _testCourse!.Id;

        // Act
        var response = await _client.GetAsync($"/v1/content/course/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CourseResponse>();
        result.Should().NotBeNull();
        result!.Should().NotBeNull();
        result.Id.Should().Be(courseId);
        result.Title.Should().Be(_testCourse.Title);
        result.Description.Should().Be(_testCourse.Description);
        result.Price.Should().Be(_testCourse.Price);
        result.IsPublished.Should().Be(_testCourse.IsPublished);
    }

    [Fact]
    public async Task GetCourseById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var courseId = Guid.CreateVersion7();

        // Act
        var response = await _client.GetAsync($"/v1/content/course/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllCourses_ShouldReturnCourses()
    {
        // Act
        var response = await _client.GetAsync("/v1/content/course/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CourseGetAll.Response>();
        result.Should().NotBeNull();
        result!.Courses.Should().NotBeEmpty();
        result.Courses.Should().Contain(c => c.Id == _testCourse!.Id);
    }

    [Fact]
    public async Task CreateLesson_WithValidRequest_ShouldCreateLesson()
    {
        // Arrange
        var command = new LessonCreate.Command
        {
            Title = "Test Lesson",
            Description = "Test Description",
            VideoUrl = "https://example.com/video",
            Duration = TimeSpan.FromMinutes(30),
            Files = new List<LessonFileRequest>()
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/content/course/{_testCourse!.Id}/lesson", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LessonCreate.Response>();
        result.Should().NotBeNull();
        result!.LessonId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateLesson_WithInvalidRequest_ShouldReturnValidationError()
    {
        // Arrange
        var command = new LessonCreate.Command
        {
            Title = "", // Invalid title
            Description = "Test Description",
            VideoUrl = "https://example.com/video",
            Duration = TimeSpan.FromMinutes(30),
            Files = new List<LessonFileRequest>()
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/content/course/{_testCourse!.Id}/lesson", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        result.Should().NotBeNull();
        result.Errors.Any(e => e.Value.Any(x => x.Contains("The Title field is required."))).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllLessons_ShouldReturnLessons()
    {
        // Act
        var response = await _client.GetAsync($"/v1/content/course/{_testCourse!.Id}/lesson");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LessonGetAll.Response>();
        result.Should().NotBeNull();
        result!.Lessons.Should().NotBeEmpty();
        result.Lessons.Should().AllSatisfy(l => l.Id.Should().NotBe(Guid.Empty));
    }
} 