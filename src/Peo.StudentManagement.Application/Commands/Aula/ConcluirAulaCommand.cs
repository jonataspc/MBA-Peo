using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.Aula;

public class ConcluirAulaCommand : IRequest<Result<ProgressoAulaResponse>>
{
    public ConcluirAulaRequest Request { get; }

    public ConcluirAulaCommand(ConcluirAulaRequest request)
    {
        Request = request;
    }
}