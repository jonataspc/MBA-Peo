using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests
{
    public record CourseEnrollmentRequest(
        [Required]
        Guid CourseId
        );
}