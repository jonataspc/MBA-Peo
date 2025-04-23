using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
using Peo.Core.Entities.Base;

namespace Peo.ContentManagement.Domain.Entities
{
    public class Course : EntityBase, IAggregateRoot
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public virtual User? Instructor { get; set; }
        public required Guid InstructorId { get; set; }
        public virtual ProgramContent? ProgramContent { get; set; }
        public decimal Price { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public virtual List<string> Tags { get; set; } = [];
        public virtual List<Lesson> Lessons { get; set; } = [];

        public Course()
        {
        }

        public Course(string title, string? description, Guid instructorId, ProgramContent? programContent, decimal price, bool isPublished, DateTime? publishedAt, List<string> tags, List<Lesson> lessons)
        {
            Title = title;
            Description = description;
            InstructorId = instructorId;
            ProgramContent = programContent;
            Price = price;
            IsPublished = isPublished;
            PublishedAt = publishedAt;
            Tags = tags;
            Lessons = lessons;
        }
    }
}