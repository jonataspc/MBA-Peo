using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.ContentManagement.Application.UseCases.Curso.ObterPorId;

public sealed record Query(Guid CursoId) : IRequest<Result<Response>>;