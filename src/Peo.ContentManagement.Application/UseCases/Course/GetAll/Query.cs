using MediatR;
using Peo.Core.DomainObjects.Result;

namespace Peo.ContentManagement.Application.UseCases.Course.GetAll;

public sealed record Query() : IRequest<Result<Response>>;