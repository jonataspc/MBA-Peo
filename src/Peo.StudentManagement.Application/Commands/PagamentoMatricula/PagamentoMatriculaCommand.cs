using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.PagamentoMatricula;

public class PagamentoMatriculaCommand : IRequest<Result<PagamentoMatriculaResponse>>
{
    public PagamentoMatriculaRequest Request { get; }

    public PagamentoMatriculaCommand(PagamentoMatriculaRequest request)
    {
        Request = request;
    }
}