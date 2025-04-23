namespace Peo.ContentManagement.Application.Dtos
{
    public class LessonResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string VideoUrl { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public IEnumerable<LessonFileResponse> Files { get; set; } = [];
    }
}