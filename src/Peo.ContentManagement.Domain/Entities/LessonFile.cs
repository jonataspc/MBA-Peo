using Peo.Core.Entities.Base;

namespace Peo.ContentManagement.Domain.Entities
{
    public class LessonFile : EntityBase
    {
        public required string Title { get; set; }
        public required string Url { get; set; } 
    }
}
