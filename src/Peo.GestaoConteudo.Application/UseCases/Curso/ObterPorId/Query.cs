using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.ObterPorId;

public sealed record Query(Guid CursoId) : IRequest<Result<Response>>;