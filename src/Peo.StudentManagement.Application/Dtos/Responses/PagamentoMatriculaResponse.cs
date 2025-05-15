using Peo.Billing.Domain.ValueObjects;

namespace Peo.StudentManagement.Application.Dtos.Responses;

public record PagamentoMatriculaResponse(
    Guid MatriculaId,
    string StatusPagamento,
    decimal ValorPago
);