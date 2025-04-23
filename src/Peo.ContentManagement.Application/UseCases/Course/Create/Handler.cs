using MediatR;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;

namespace Peo.ContentManagement.Application.UseCases.Course.Create;

public class Handler(IRepository<Domain.Entities.Course> repository) : IRequestHandler<Command, Result<Response>>
{
    public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
    {
        var course = new Domain.Entities.Course(

            title: request.Title,
            description: request.Description,
            instructorId: request.InstructorId,
            programContent: new ProgramContent(request.ProgramContent),
            price: request.Price,
            isPublished: true,
            publishedAt: DateTime.Now,
            tags: request.Tags ?? [],
            lessons: []
        );

        repository.Insert(course);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new Response(course.Id));
    }
}