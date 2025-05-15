using Mapster;
using MediatR;
using Peo.ContentManagement.Application.Dtos;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;

namespace Peo.ContentManagement.Application.UseCases.Curso.ObterPorId;

public class Handler(IRepository<Domain.Entities.Curso> repository) : IRequestHandler<Query, Result<Response>>
{
    public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cursos = await repository.GetAsync(request.CursoId);
        return Result.Success(new Response(cursos.Adapt<CursoResponse>()));
    }
}