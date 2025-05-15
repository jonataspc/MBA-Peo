using FluentAssertions;
using NetArchTest.Rules;
using Peo.Core.Interfaces.Data;
using Peo.Tests.ArchitectureTests.Extensions;
using System.Reflection;

namespace Peo.Tests.ArchitectureTests;

public class ArchitectTests
{
    private static IEnumerable<Assembly> GetDomainAssemblies()
    {
        return [
            typeof(Peo.Identity.Domain.Interfaces.Data.IUserRepository ).Assembly,
            typeof(Peo.Billing.Domain.Dtos.CartaoCredito).Assembly,
            typeof(Peo.ContentManagement.Domain.Entities.Curso).Assembly,
            typeof(Peo.StudentManagement.Domain.Entities.Certificado ).Assembly,
            typeof(Peo.Core.DomainObjects.AccessRoles).Assembly,
            ];
    }

    private static IEnumerable<Assembly> GetAllAssemblies()
    {
        var list = new List<Assembly>
        {
            typeof(Peo.Web.Api.Services.AppIdentityUser).Assembly,
            typeof(Peo.Core.Web.Api.IEndpoint).Assembly,
            typeof(Peo.Core.Infra.Data.Contexts.Base.DbContextBase ).Assembly,
            typeof(Peo.StudentManagement.Infra.Data.Helpers.StudentManagementeDbMigrationHelpers).Assembly,
            typeof(Peo.StudentManagement.Application.Endpoints.EndpointsEstudante ).Assembly,
            typeof(Peo.Identity.Infra.Data.Helpers.IdentityDbMigrationHelpers).Assembly,
            typeof(Peo.Identity.Application.Services.TokenService).Assembly,
            typeof(Peo.ContentManagement.Infra.Data.Helpers.GestaoConteudoDbMigrationHelpers).Assembly,
            typeof(Peo.ContentManagement.Application.Services.CursoAulaService).Assembly,
            typeof(Peo.Billing.Integrations.Paypal.Services .PaypalBrokerService ).Assembly,
            typeof(Peo.Billing.Infra.Data.Helpers.BillingDbMigrationHelpers).Assembly,
            typeof(Peo.Billing.Application.Services.PagamentoService ).Assembly,
        };

        list.AddRange(GetDomainAssemblies());

        return list;
    }

    [Fact]
    public void Domain_Must_Not_Reference_External_Libraries()
    {
        var result = Types
            .InAssemblies(GetDomainAssemblies())
            .Should()
            .OnlyHaveDependencyOn(
            "System",
            "Microsoft",
            "Peo.Identity.Domain",
            "Peo.Core",
            "Peo.Billing.Domain",
            "Peo.ContentManagement.Domain",
            "Peo.StudentManagement.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }

    [Fact]
    public void DataAccess_Must_Reside_In_Infra()
    {
        var result = Types.InAssemblies(GetAllAssemblies())
          .That()
          .HaveDependencyOnAll("Microsoft.EntityFrameworkCore")
          .And()
          .ResideInNamespace("Peo")
          .Should()
          .ResideInNamespaceMatching(@"^Peo.*Infra\.Data.*")
          .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }

    [Fact]
    public void Repositories_Must_Have_Name_Ending_In_Repository()
    {
        var result = Types.InAssemblies(GetAllAssemblies())
          .That()
          .AreClasses()
          .And()
          .ImplementInterface(typeof(IRepository<>))
          .Should()
          .HaveNameEndingWith("Repository")
          .And()
          .ResideInNamespaceMatching(@"^Peo.*Infra\.Data.*")
          .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }

    [Fact]
    public void Interfaces_Must_Start_Whith_I()
    {
        var result = Types.InAssemblies(GetAllAssemblies())
        .That()
        .AreInterfaces()
        .Should()
        .HaveNameStartingWith("I")
        .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }
}