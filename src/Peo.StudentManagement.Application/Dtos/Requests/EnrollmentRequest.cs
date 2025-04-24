using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests
{
    public record EnrollmentRequest(
        [Required]
        Guid CourseId
        );
}