using Mapster;
using MediatR;
using Peo.ContentManagement.Application.Dtos;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;

namespace Peo.ContentManagement.Application.UseCases.Lesson.GetAll;

public class Handler(IRepository<Domain.Entities.Course> repository) : IRequestHandler<Query, Result<Response>>
{
    public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        var course = await repository.GetAsync(request.CourseId);

        if (course is null)
        {
            return Result.Failure<Response>(new Error(null!, "Course does not exist"));
        }

        return Result.Success<Response>(new Response(course.Lessons.Adapt<IEnumerable<LessonResponse>>()));
    }
}