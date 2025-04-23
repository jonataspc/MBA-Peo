using Peo.Core.Entities.Base;

namespace Peo.ContentManagement.Domain.Entities
{
    public class Lesson : EntityBase
    {
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public string VideoUrl { get; private set; } = null!;
        public TimeSpan Duration { get; private set; }
        public virtual ICollection<LessonFile> Files { get; private set; } = [];

        public virtual Course Course { get; private set; } = null!;
        public Guid CourseId { get; private set; }
                

        public Lesson()
        {
        }

        public Lesson(string title, string? description, string videoUrl, TimeSpan duration, ICollection<LessonFile> files, Guid courseId)
        {
            Title = title;
            Description = description;
            VideoUrl = videoUrl;
            Duration = duration;
            Files = files;
            CourseId = courseId;
        }
    }
}