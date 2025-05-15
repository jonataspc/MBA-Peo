using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests;

public class ConcluirAulaRequest
{
    [Required]
    public Guid MatriculaId { get; set; }

    [Required]
    public Guid AulaId { get; set; }
}