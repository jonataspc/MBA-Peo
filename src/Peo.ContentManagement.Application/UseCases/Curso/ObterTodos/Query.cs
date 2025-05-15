using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.ContentManagement.Application.UseCases.Curso.ObterTodos;

public sealed record Query() : IRequest<Result<Response>>;