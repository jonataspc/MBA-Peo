using Peo.ContentManagement.Application.Dtos;

namespace Peo.ContentManagement.Application.UseCases.Aula.ObterTodos;

public sealed record Response(IEnumerable<AulaResponse> Aulas);