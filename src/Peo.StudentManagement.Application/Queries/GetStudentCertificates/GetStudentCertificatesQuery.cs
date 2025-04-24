using Peo.StudentManagement.Application.Dtos.Responses;

namespace Peo.StudentManagement.Application.Queries.GetStudentCertificates;

public class GetStudentCertificatesQuery : IRequest<Result<IEnumerable<StudentCertificateResponse>>>
{
    public GetStudentCertificatesQuery()
    {
    }
}