using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.GestaoAlunos.Application.Dtos.Responses;

namespace Peo.GestaoAlunos.Application.Queries.ObterCertificadosEstudante;

public class ObterCertificadosEstudanteQuery : IRequest<Result<IEnumerable<CertificadoEstudanteResponse>>>
{
    public ObterCertificadosEstudanteQuery()
    {
    }
}