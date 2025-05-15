using FluentAssertions;
using Moq;
using Peo.ContentManagement.Application.UseCases.Curso.Cadastrar;
using Peo.Core.Interfaces.Data;

namespace Peo.Tests.UnitTests.ContentManagement.Course;

public class CadastrarCursoCommandHandlerTests
{
    private readonly Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public CadastrarCursoCommandHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.ContentManagement.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveCadastrarCurso_QuandoValido()
    {
        // Arrange
        var comando = new Command(
            Titulo: "Curso Teste",
            Descricao: "Descrição Teste",
            InstrutorId: Guid.CreateVersion7(),
            ConteudoProgramatico: "Conteúdo Programático Teste",
            Preco: 99.99m,
            Tags: ["teste", "curso"]
        );

        _repositorioMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.CursoId.Should().NotBeEmpty();

        _repositorioMock.Verify(x => x.Insert(It.Is<Peo.ContentManagement.Domain.Entities.Curso>(c =>
            c.Titulo == comando.Titulo &&
            c.Descricao == comando.Descricao &&
            c.InstrutorId == comando.InstrutorId &&
            c.ConteudoProgramatico!.Conteudo == comando.ConteudoProgramatico &&
            c.Preco == comando.Preco &&
            c.Tags.SequenceEqual(comando.Tags!)
        )), Times.Once);

        _repositorioMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}