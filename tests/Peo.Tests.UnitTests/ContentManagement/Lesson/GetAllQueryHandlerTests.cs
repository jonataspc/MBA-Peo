using FluentAssertions;
using Mapster;
using Moq;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Application.UseCases.Aula.ObterTodos;
using Peo.ContentManagement.Domain.ValueObjects;
using Peo.Core.Interfaces.Data;

namespace Peo.Tests.UnitTests.ContentManagement.Lesson;

public class ObterTodasAulasQueryHandlerTests
{
    private readonly Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public ObterTodasAulasQueryHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveRetornarTodasAulas_QuandoCursoExiste()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var aulas = new List<Peo.ContentManagement.Domain.Entities.Aula>
        {
            new(
                titulo: "Aula Teste 1",
                descricao: "Descrição da Aula Teste 1",
                urlVideo: "https://example.com/video1",
                duracao: TimeSpan.FromMinutes(30),
                arquivos: new List<Peo.ContentManagement.Domain.Entities.ArquivoAula>(),
                cursoId: cursoId
            ),
            new(
                titulo: "Aula Teste 2",
                descricao: "Descrição da Aula Teste 2",
                urlVideo: "https://example.com/video2",
                duracao: TimeSpan.FromMinutes(45),
                arquivos: new List<Peo.ContentManagement.Domain.Entities.ArquivoAula>(),
                cursoId: cursoId
            )
        };

        var curso = new Peo.ContentManagement.Domain.Entities.Curso(
            titulo: "Curso Teste",
            descricao: "Descrição Teste",
            instrutorId: Guid.CreateVersion7(),
            conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste"),
            preco: 99.99m,
            estaPublicado: true,
            dataPublicacao: DateTime.Now,
            tags: new List<string> { "teste", "curso" },
            aulas: aulas
        );

        _repositorioMock.Setup(x => x.GetAsync(cursoId))
            .ReturnsAsync(curso);

        var consulta = new Query(cursoId);

        // Act
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Aulas.Should().NotBeNull();
        resultado.Value.Aulas.First().Arquivos.Should().NotBeNull();
        resultado.Value.Aulas.Should().HaveCount(2);
        resultado.Value.Aulas.Should().BeEquivalentTo(aulas.Adapt<IEnumerable<AulaResponse>>());
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
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be("Curso não existe");
    }

    [Fact]
    public async Task Handler_DeveRetornarListaVazia_QuandoCursoNaoTemAulas()
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
        resultado.Value.Aulas.Should().NotBeNull();
        resultado.Value.Aulas.Should().BeEmpty();
    }
}