using FluentAssertions;
using Mapster;
using Moq;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Application.UseCases.Curso.ObterPorId;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.DomainObjects.Result;
using Peo.Core.Interfaces.Data;

namespace Peo.Tests.UnitTests.ContentManagement.Course;

public class ObterCursoPorIdQueryHandlerTests
{
    private readonly Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public ObterCursoPorIdQueryHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveRetornarCurso_QuandoExiste()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var curso = new Peo.ContentManagement.Domain.Entities.Curso(
            titulo: "Curso Teste",
            descricao: "Descrição Teste",
            instrutorId: Guid.CreateVersion7(),
            conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste"),
            preco: 99.99m,
            estaPublicado: true,
            dataPublicacao: DateTime.Now,
            tags: new List<string> { "teste", "curso" },
            aulas: new List<Peo.ContentManagement.Domain.Entities.Aula>()
        );

        _repositorioMock.Setup(x => x.GetAsync(cursoId))
            .ReturnsAsync(curso);

        var consulta = new Query(cursoId);

        // Act
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Curso.Should().NotBeNull();
        resultado.Value.Curso.Should().BeEquivalentTo(curso.Adapt<CursoResponse>());
    }

    [Fact]
    public async Task Handler_DeveRetornarFalha_QuandoCursoNaoEncontrado()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        _repositorioMock.Setup(x => x.GetAsync(cursoId))
            .ReturnsAsync((Peo.ContentManagement.Domain.Entities.Curso?)null);

        var consulta = new Query(cursoId);

        // Act
        var result = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Curso.Should().BeNull();
    }
}