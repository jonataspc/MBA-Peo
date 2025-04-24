using Microsoft.Extensions.Logging;
using Peo.Core.Interfaces.Services;
using Peo.StudentManagement.Application.Dtos.Responses;
using Peo.StudentManagement.Domain.Interfaces;

namespace Peo.StudentManagement.Application.Queries.GetStudentCertificates;

public class GetStudentCertificatesQueryHandler : IRequestHandler<GetStudentCertificatesQuery, Result<IEnumerable<StudentCertificateResponse>>>
{
    private readonly IStudentService _studentService;
    private readonly ILogger<GetStudentCertificatesQueryHandler> _logger;
    private readonly IAppIdentityUser _appIdentityUser;

    public GetStudentCertificatesQueryHandler(IStudentService studentService, ILogger<GetStudentCertificatesQueryHandler> logger, IAppIdentityUser appIdentityUser)
    {
        _studentService = studentService;
        _logger = logger;
        _appIdentityUser = appIdentityUser;
    }

    public async Task<Result<IEnumerable<StudentCertificateResponse>>> Handle(GetStudentCertificatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentService.GetStudentByUserId(_appIdentityUser.GetUserId(), cancellationToken);
            var certificates = await _studentService.GetStudentCertificatesAsync(student.Id, cancellationToken);

            var response = certificates.Select(c => new StudentCertificateResponse(
                c.Id,
                c.EnrollmentId,
                c.Content,
                c.IssueDate,
                c.CertificateNumber
            ));

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting certificates for student");
            return Result.Failure<IEnumerable<StudentCertificateResponse>>(new Error(ex.Message));
        }
    }
}