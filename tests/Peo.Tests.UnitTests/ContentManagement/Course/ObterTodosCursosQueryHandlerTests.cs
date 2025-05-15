using FluentAssertions;
using Mapster;
using Moq;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Application.UseCases.Curso.ObterTodos;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.Interfaces.Data;

namespace Peo.Tests.UnitTests.ContentManagement.Course;

public class ObterTodosCursosQueryHandlerTests
{
    private readonly Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public ObterTodosCursosQueryHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveRetornarTodosCursos()
    {
        // Arrange
        var cursos = new List<Peo.ContentManagement.Domain.Entities.Curso>
        {
            new(
                titulo: "Curso Teste 1",
                descricao: "Descrição Teste 1",
                instrutorId: Guid.CreateVersion7(),
                conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste 1"),
                preco: 99.99m,
                estaPublicado: true,
                dataPublicacao: DateTime.Now,
                tags: new List<string> { "teste", "curso" },
                aulas: new List<Peo.ContentManagement.Domain.Entities.Aula>()
            ),
            new(
                titulo: "Curso Teste 2",
                descricao: "Descrição Teste 2",
                instrutorId: Guid.CreateVersion7(),
                conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste 2"),
                preco: 199.99m,
                estaPublicado: true,
                dataPublicacao: DateTime.Now,
                tags: new List<string> { "teste", "curso" },
                aulas: new List<Peo.ContentManagement.Domain.Entities.Aula>()
            )
        };

        _repositorioMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(cursos);

        var consulta = new Query();

        // Act
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Cursos.Should().NotBeNull();
        resultado.Value.Cursos.Should().HaveCount(2);
        resultado.Value.Cursos.Should().BeEquivalentTo(cursos.Adapt<IEnumerable<CursoResponse>>());
    }

    [Fact]
    public async Task Handler_DeveRetornarListaVazia_QuandoNaoExistemCursos()
    {
        // Arrange
        _repositorioMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<Peo.ContentManagement.Domain.Entities.Curso>());

        var consulta = new Query();

        // Act
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Cursos.Should().NotBeNull();
        resultado.Value.Cursos.Should().BeEmpty();
    }
}