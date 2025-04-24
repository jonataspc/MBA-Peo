using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.Lesson;

public class StartLessonCommand : IRequest<Result<LessonProgressResponse>>
{
    public StartLessonRequest Request { get; }

    public StartLessonCommand(StartLessonRequest request)
    {
        Request = request;
    }
}