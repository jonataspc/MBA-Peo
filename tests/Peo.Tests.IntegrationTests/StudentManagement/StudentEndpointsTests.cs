using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using Peo.Billing.Domain.Dtos;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.DomainObjects.Result;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Entities;
using Peo.Tests.IntegrationTests.Setup;
using System.Net;
using System.Net.Http.Json;

namespace Peo.Tests.IntegrationTests.StudentManagement;

public class StudentEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestDatabaseSetup _testDb;
    private Guid _testUserId = Guid.CreateVersion7();
    private Course _testCourse = null!;
    private Course _testCourseNotEnrolled = null!;
    private Student? _testStudent;
    private Enrollment? _testEnrollment;
    private Enrollment? _testEnrollmentNotPaid;

    public StudentEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _testDb = new TestDatabaseSetup(_factory.Services);
    }

    public async Task InitializeAsync()
    {
        // Create test student
        _testStudent = await _testDb.CreateTestStudentAsync(_testUserId);

        // COurse
        var course = await _testDb.CreateTestCourseAsync(instructorId: _testStudent.UserId);
        _testCourse = course;

        var courseTwo = await _testDb.CreateTestCourseAsync(instructorId: _testStudent.UserId);


        var courseNotEnrolled = await _testDb.CreateTestCourseAsync(instructorId: _testStudent.UserId);
        _testCourseNotEnrolled = courseNotEnrolled;

        // Create test enrollment
        _testEnrollment = await _testDb.CreateTestEnrollmentAsync(_testStudent.Id, _testCourse.Id, true);
        _testEnrollmentNotPaid = await _testDb.CreateTestEnrollmentAsync(_testStudent.Id, courseTwo.Id, false);



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
    public async Task GetStudentCertificates_ShouldReturnCertificates()
    {
        // Arrange
        await _testDb.CreateTestCertificateAsync(
            _testEnrollment!.Id,
            "Test Certificate Content");

        // Act
        var response = await _client.GetAsync("/v1/student/certificates");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var certificates = await response.Content.ReadFromJsonAsync<IEnumerable<StudentCertificateResponse>>();
        certificates.Should().NotBeNull();
        certificates.Should().Contain(c => c.EnrollmentId == _testEnrollment.Id);
    }

    [Fact]
    public async Task StartLesson_WithValidRequest_ShouldStartLesson()
    {
        // Arrange
        var request = new StartLessonRequest
        {
            EnrollmentId = _testEnrollment!.Id,
            LessonId = _testCourse.Lessons.First().Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment/lesson/start", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LessonProgressResponse>();
        result.Should().NotBeNull();
        result!.EnrollmentId.Should().Be(request.EnrollmentId);
        result.LessonId.Should().Be(request.LessonId);
        result.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task EndLesson_WithValidRequest_ShouldEndLesson()
    {
        // Arrange
        var lessonId = Guid.CreateVersion7();
        await _testDb.CreateTestLessonProgressAsync(_testEnrollment!.Id, lessonId);

        var request = new EndLessonRequest
        {
            EnrollmentId = _testEnrollment.Id,
            LessonId = lessonId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment/lesson/end", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LessonProgressResponse>();
        result.Should().NotBeNull();
        result!.EnrollmentId.Should().Be(request.EnrollmentId);
        result.LessonId.Should().Be(request.LessonId);
        result.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task CourseEnrollment_WithValidRequest_WhenAlreadEnrolledInACourse()
    {
        // Arrange
        var request = new CourseEnrollmentRequest(_testCourse.Id);

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Contain("Student is alread enrolled in this course");

    }

    [Fact]
    public async Task CourseEnrollment_WithValidRequest_ShouldCreateEnrollment()
    {
        // Arrange
        var request = new CourseEnrollmentRequest(_testCourseNotEnrolled.Id);

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CourseEnrollmentResponse>();
        result.Should().NotBeNull();
        result!.EnrollmentId.Should().NotBe(Guid.Empty);
    }


    [Fact]
    public async Task CompleteEnrollment_WithValidRequest_ShouldCompleteEnrollment()
    {
        // Arrange

        // End all lessons
        foreach (var lesson in _testCourse.Lessons)
        {
            var requestLessonStart = new StartLessonRequest
            {
                EnrollmentId = _testEnrollment!.Id,
                LessonId = lesson.Id
            };

            await _client.PostAsJsonAsync("/v1/student/enrollment/lesson/start", requestLessonStart);

            var requestLessonEnd = new EndLessonRequest
            {
                EnrollmentId = _testEnrollment.Id,
                LessonId = lesson.Id
            };
            await _client.PostAsJsonAsync("/v1/student/enrollment/lesson/end", requestLessonEnd);
        }


        var request = new CompleteEnrollmentRequest
        {
            EnrollmentId = _testEnrollment!.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment/complete", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CompleteEnrollmentResponse>();
        result.Should().NotBeNull();
        result!.EnrollmentId.Should().Be(request.EnrollmentId);
        result.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task EnrollmentPayment_WithValidRequest_ShouldProcessPayment()
    {
        // Arrange
        var request = new EnrollmentPaymentRequest(
            _testEnrollmentNotPaid!.Id,
            new CreditCard(
                "4111111111111111",
                "12/25",
                "123",
                "John Doe"
            )
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment/payment", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<EnrollmentPaymentResponse>();
        result.Should().NotBeNull();
        result!.EnrollmentId.Should().Be(_testEnrollmentNotPaid.Id);
        result.Status.Should().Be(Billing.Domain.ValueObjects.PaymentStatus.Paid);
    }

    [Fact]
    public async Task EnrollmentPayment_WithInvalidEnrollmentId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new EnrollmentPaymentRequest(
            Guid.CreateVersion7(), // Non-existent enrollment ID
            new CreditCard(
                "4111111111111111",
                "12/25",
                "123",
                "John Doe"
            )
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment/payment", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Enrollment not found");
    }

    [Fact]
    public async Task EnrollmentPayment_WithInvalidCreditCard_ShouldReturnValidationError()
    {
        // Arrange
        var request = new EnrollmentPaymentRequest(
            _testEnrollmentNotPaid!.Id,
            new CreditCard(
                "1234", // Invalid card number
                "12/25",
                "123",
                "John Doe"
            )
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment/payment", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Credit card is invalid");
    }

    [Fact]
    public async Task EnrollmentPayment_WithMissingRequiredFields_ShouldReturnValidationError()
    {
        // Arrange
        var request = new EnrollmentPaymentRequest(
            _testEnrollmentNotPaid!.Id,
            new CreditCard(
                null, // Missing card number
                "12/25",
                "123",
                "John Doe"
            )
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/student/enrollment/payment", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Credit card is null");
    }
}