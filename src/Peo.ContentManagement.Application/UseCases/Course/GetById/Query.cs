using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.ContentManagement.Application.UseCases.Course.GetById;

public sealed record Query(Guid CourseId) : IRequest<Result<Response>>;