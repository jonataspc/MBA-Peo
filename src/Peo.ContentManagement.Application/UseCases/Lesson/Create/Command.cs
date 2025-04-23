using MediatR;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.DomainObjects.Result;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peo.ContentManagement.Application.UseCases.Lesson.Create;

public sealed class Command : IRequest<Result<Response>>
{
    [NotMapped]
    public Guid CourseId { get; set; }

    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    public required string VideoUrl { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    public IEnumerable<LessonFileRequest> Files { get; set; } = [];
}