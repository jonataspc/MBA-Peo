using MediatR;
using Peo.Core.DomainObjects.Result;
using System.ComponentModel.DataAnnotations;

namespace Peo.ContentManagement.Application.UseCases.Course.Create;

public sealed record Command(


    [Required]
    string Title,

    string? Description,

    [Required]
    Guid InstructorId,

    string? ProgramContent,

    [Required]
    decimal Price,

    List<string>? Tags)

    : IRequest<Result<Response>>;
