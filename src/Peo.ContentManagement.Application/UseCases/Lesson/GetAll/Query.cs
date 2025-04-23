using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.ContentManagement.Application.UseCases.Lesson.GetAll;

public sealed record Query(Guid CourseId) : IRequest<Result<Response>>;
