using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.Lesson;

public class EndLessonCommand : IRequest<Result<LessonProgressResponse>>
{
    public EndLessonRequest Request { get; }

    public EndLessonCommand(EndLessonRequest request)
    {
        Request = request;
    }
}