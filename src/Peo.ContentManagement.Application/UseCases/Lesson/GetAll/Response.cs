using Peo.ContentManagement.Application.Dtos;

namespace Peo.ContentManagement.Application.UseCases.Lesson.GetAll;

public sealed record Response(IEnumerable<LessonResponse> Lessons);