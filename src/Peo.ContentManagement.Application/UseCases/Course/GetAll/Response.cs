using Peo.ContentManagement.Application.Dtos;

namespace Peo.ContentManagement.Application.UseCases.Course.GetAll;

public sealed record Response(IEnumerable<CourseResponse> Courses);