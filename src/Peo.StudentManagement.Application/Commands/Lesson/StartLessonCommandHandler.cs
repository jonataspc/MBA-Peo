using Microsoft.Extensions.Logging;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Commands.Lesson;

public class StartLessonCommandHandler : IRequestHandler<StartLessonCommand, Result<LessonProgressResponse>>
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StartLessonCommandHandler> _logger;

    public StartLessonCommandHandler(IStudentService studentService, ILogger<StartLessonCommandHandler> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    public async Task<Result<LessonProgressResponse>> Handle(StartLessonCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var progress = await _studentService.StartLessonAsync(request.Request.EnrollmentId, request.Request.LessonId, cancellationToken);

            var response = new LessonProgressResponse(progress.EnrollmentId, progress.LessonId, progress.IsCompleted, progress.StartedAt, progress.CompletedAt, await _studentService.GetCourseOverallProgress(progress.EnrollmentId, cancellationToken));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting lesson for enrollment {EnrollmentId} and lesson {LessonId}",
                request.Request.EnrollmentId, request.Request.LessonId);
            return Result.Failure<LessonProgressResponse>(new Error(ex.Message));
        }
    }
}