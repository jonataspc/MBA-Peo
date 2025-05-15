using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests;

public class IniciarAulaRequest
{
    [Required]
    public Guid MatriculaId { get; set; }

    [Required]
    public Guid AulaId { get; set; }
}