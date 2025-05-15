using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Queries.GetStudentCertificates;

public class ObterCertificadosEstudanteQuery : IRequest<Result<IEnumerable<CertificadoEstudanteResponse>>>
{
    public ObterCertificadosEstudanteQuery()
    {
    }
}