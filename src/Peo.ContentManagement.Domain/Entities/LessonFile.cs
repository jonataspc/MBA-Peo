using Peo.Core.Entities.Base;

namespace Peo.ContentManagement.Domain.Entities
{
    public class LessonFile : EntityBase
    {
        public string Title { get; private set; } = null!;
        public string Url { get; private set; } = null!;

        public virtual Lesson? Lesson { get; private set; } 
        public Guid LessonId { get; private set; }
        

        public LessonFile()
        {
        }

        public LessonFile(string title, string url, Guid lessonId)
        {
            Title = title;
            Url = url;
            LessonId = lessonId;
        }
    }
}