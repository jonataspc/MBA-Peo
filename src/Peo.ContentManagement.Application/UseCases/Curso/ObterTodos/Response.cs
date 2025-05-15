using Peo.ContentManagement.Application.Dtos;

namespace Peo.ContentManagement.Application.UseCases.Curso.ObterTodos;

public sealed record Response(IEnumerable<CursoResponse> Cursos);