using Mapster;
using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;

namespace Peo.ContentManagement.Application.UseCases.Lesson.Create;

public class Handler(IRepository<Domain.Entities.Course> repository) : IRequestHandler<Command, Result<Response>>
{
    public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
    {
        var course = await repository.WithTracking()
                                     .GetAsync(request.CourseId);

        if (course is null)
        {
            return Result.Failure<Response>(new Error("Course not found"));
        }

        var lesson = request.Adapt<Domain.Entities.Lesson>();

        course.Lessons.Add(lesson);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new Response(lesson.Id));
    }
}