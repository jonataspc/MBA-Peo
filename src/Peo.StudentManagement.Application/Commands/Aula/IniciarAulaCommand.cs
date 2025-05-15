using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.Aula;

public class IniciarAulaCommand : IRequest<Result<ProgressoAulaResponse>>
{
    public IniciarAulaRequest Request { get; }

    public IniciarAulaCommand(IniciarAulaRequest request)
    {
        Request = request;
    }
}