namespace Peo.ContentManagement.Application.Dtos
{
    public class CourseResponse
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? InstructorName { get; set; }
        public Guid InstructorId { get; set; }
        public string ProgramContent { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public IEnumerable<string> Tags { get; set; } = [];
        public IEnumerable<LessonResponse> Lessons { get; set; } = [];
    }
}