using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.Matricula;

public class ConcluirMatriculaCommand : IRequest<Result<ConcluirMatriculaResponse>>
{
    public ConcluirMatriculaRequest Request { get; }

    public ConcluirMatriculaCommand(ConcluirMatriculaRequest request)
    {
        Request = request;
    }
}