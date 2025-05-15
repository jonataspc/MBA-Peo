using FluentAssertions;
using Moq;
using Peo.Core.Interfaces.Services.Acls;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.Interfaces.Services;
using Peo.Faturamento.Domain.ValueObjects;
using Peo.GestaoAlunos.Application.Commands.PagamentoMatricula;
using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class PagamentoMatriculaCommandHandlerTests
{
    private readonly Mock<IEstudanteRepository> _estudanteRepositoryMock;
    private readonly Mock<IPagamentoService> _pagamentoServiceMock;
    private readonly Mock<ICursoAulaService> _courseLessonServiceMock;
    private readonly PagamentoMatriculaCommandHandler _handler;

    public PagamentoMatriculaCommandHandlerTests()
    {
        _estudanteRepositoryMock = new Mock<IEstudanteRepository>();
        _pagamentoServiceMock = new Mock<IPagamentoService>();
        _courseLessonServiceMock = new Mock<ICursoAulaService>();
        _handler = new PagamentoMatriculaCommandHandler(
            _estudanteRepositoryMock.Object,
            _pagamentoServiceMock.Object,
            _courseLessonServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagamento_QuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, cursoId);
        var valor = 99.99m;
        var cartaoCredito = new CartaoCredito("1234567890123456", "12/25", "123", "Usuário Teste");
        var pagamento = new Pagamento(matriculaId, valor);
        pagamento.ProcessarPagamento(Guid.CreateVersion7().ToString());
        pagamento.ConfirmarPagamento(new CartaoCreditoData { Hash = "hash-123" });

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _courseLessonServiceMock.Setup(x => x.ObterPrecoCursoAsync(cursoId))
            .ReturnsAsync(valor);
        _pagamentoServiceMock.Setup(x => x.ProcessarPagamentoMatriculaAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<CartaoCredito>()))
            .ReturnsAsync(pagamento);

        var requisicao = new PagamentoMatriculaRequest
        {
            MatriculaId = matriculaId,
            DadosCartao = cartaoCredito
        };
        var comando = new PagamentoMatriculaCommand(requisicao);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.MatriculaId.Should().Be(matriculaId);
        resultado.Value.StatusPagamento.Should().Be(StatusPagamento.Pago.ToString());
        resultado.Value.ValorPago.Should().Be(valor);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoOcorreErro()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, cursoId);
        var valor = 99.99m;
        var mensagemErro = "Falha ao processar o pagamento";

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _courseLessonServiceMock.Setup(x => x.ObterPrecoCursoAsync(cursoId))
            .ReturnsAsync(valor);
        _pagamentoServiceMock.Setup(x => x.ProcessarPagamentoMatriculaAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<CartaoCredito>()))
            .ThrowsAsync(new Exception(mensagemErro));

        var requisicao = new PagamentoMatriculaRequest
        {
            MatriculaId = matriculaId,
            DadosCartao = new("1234567890123456", "Usuário Teste", "12/25", "123")
        };
        var comando = new PagamentoMatriculaCommand(requisicao);

        // Act & assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(comando, CancellationToken.None));
    }
}