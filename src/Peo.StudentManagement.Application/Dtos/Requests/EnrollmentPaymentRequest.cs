using Peo.Billing.Domain.Dtos;
using Peo.Billing.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Peo.StudentManagement.Application.Dtos.Requests;

public class PagamentoMatriculaRequest
{
    [Required]
    public Guid MatriculaId { get; set; }

    [Required]
    public CartaoCredito DadosCartao { get; set; } = null!;
}