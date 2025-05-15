using Peo.StudentManagement.Application.Dtos.Requests;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Commands.MatriculaCurso;

public class MatriculaCursoCommand : IRequest<Result<MatriculaCursoResponse>>
{
    public MatriculaCursoRequest Request { get; }

    public MatriculaCursoCommand(MatriculaCursoRequest request)
    {
        Request = request;
    }
}