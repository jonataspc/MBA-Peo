using Microsoft.Extensions.Logging;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Commands.Enrollment;

public class CompleteEnrollmentCommandHandler : IRequestHandler<CompleteEnrollmentCommand, Result<CompleteEnrollmentResponse>>
{
    private readonly IStudentService _studentService;
    private readonly ILogger<CompleteEnrollmentCommandHandler> _logger;

    public CompleteEnrollmentCommandHandler(IStudentService studentService, ILogger<CompleteEnrollmentCommandHandler> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    public async Task<Result<CompleteEnrollmentResponse>> Handle(CompleteEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _studentService.CompleteEnrollmentAsync(request.Request.EnrollmentId, cancellationToken);

            var response = new CompleteEnrollmentResponse(enrollment.Id, enrollment.Status.ToString(), enrollment.CompletionDate, enrollment.ProgressPercentage);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing enrollment {EnrollmentId}", request.Request.EnrollmentId);
            return Result.Failure<CompleteEnrollmentResponse>(new Error(ex.Message));
        }
    }
}