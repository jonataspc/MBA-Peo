using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests;

public class EndLessonRequest
{
    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public Guid LessonId { get; set; }
}