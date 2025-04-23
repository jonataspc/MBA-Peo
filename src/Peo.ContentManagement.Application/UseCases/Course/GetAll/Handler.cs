using Mapster;
using MediatR;
using Peo.ContentManagement.Application.Dtos;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;

namespace Peo.ContentManagement.Application.UseCases.Course.GetAll;

public class Handler(IRepository<Domain.Entities.Course> repository) : IRequestHandler<Query, Result<Response>>
{
    public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        var courses = await repository.GetAsync(c => c.Title.Contains(request.TitleWildcard));

        return Result.Success(new Response(courses.Adapt<IEnumerable<CourseResponse>>()));
    }
}