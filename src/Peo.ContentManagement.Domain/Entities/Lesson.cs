using Peo.Core.Entities.Base;

namespace Peo.ContentManagement.Domain.Entities
{
    public class Lesson : EntityBase
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string VideoUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public virtual List<LessonFile> Files { get; set; } = [];
    }
}