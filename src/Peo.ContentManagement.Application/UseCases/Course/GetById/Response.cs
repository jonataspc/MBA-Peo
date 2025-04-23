using Peo.ContentManagement.Application.Dtos;

namespace Peo.ContentManagement.Application.UseCases.Course.GetById;

public sealed record Response(CourseResponse? Course);