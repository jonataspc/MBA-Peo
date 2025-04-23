using Peo.ContentManagement.Domain.Entities;

namespace Peo.ContentManagement.Application.Dtos
{
    public class LessonResponse
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string VideoUrl { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public IEnumerable<LessonFile> Files { get; set; } = [];
    }
}