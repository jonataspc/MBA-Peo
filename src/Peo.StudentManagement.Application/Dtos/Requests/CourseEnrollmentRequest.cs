using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests;

public class MatriculaCursoRequest
{
    [Required]
    public Guid CursoId { get; set; }
}