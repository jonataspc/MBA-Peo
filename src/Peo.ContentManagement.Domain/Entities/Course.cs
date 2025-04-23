using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
using Peo.Core.Entities.Base;

namespace Peo.ContentManagement.Domain.Entities
{
    public class Course : EntityBase, IAggregateRoot
    {
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public virtual User? Instructor { get; private set; }
        public Guid InstructorId { get; private set; }
        public virtual ProgramContent? ProgramContent { get; private set; }
        public decimal Price { get; private set; }
        public bool IsPublished { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public virtual List<string> Tags { get; private set; } = [];
        public virtual List<Lesson> Lessons { get; private set; } = [];

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