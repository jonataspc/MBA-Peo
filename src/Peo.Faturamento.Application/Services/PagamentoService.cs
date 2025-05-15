using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Data;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Domain.Entities;
using Peo.Faturamento.Domain.Interfaces.Brokers;
using Peo.Faturamento.Domain.Interfaces.Services;
using Peo.Faturamento.Domain.ValueObjects;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.Faturamento.Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IRepository<Pagamento> _pagamentoRepository;
    private readonly IEstudanteRepository _estudanteRepository;
    private readonly IBrokerPagamentoService _paymentBrokerService;

    public PagamentoService(IRepository<Pagamento> pagamentoRepository, IEstudanteRepository estudanteRepository, IBrokerPagamentoService paymentBrokerService)
    {
        _pagamentoRepository = pagamentoRepository;
        _estudanteRepository = estudanteRepository;
        _paymentBrokerService = paymentBrokerService;
    }

    private async Task<Pagamento> CriarPagamentoAsync(Guid matriculaId, decimal valor)
    {
        var pagamento = new Pagamento(matriculaId, valor);
        _pagamentoRepository.Insert(pagamento);
        await _pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    private async Task<Pagamento> ProcessarPagamentoAsync(Guid pagamentoId, string idTransacao)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.ProcessarPagamento(idTransacao);
        _pagamentoRepository.Update(pagamento);
        await _pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    public async Task<Pagamento> EstornarPagamentoAsync(Guid pagamentoId)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.Estornar();
        _pagamentoRepository.Update(pagamento);
        await _pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    public async Task<Pagamento> CancelarPagamentoAsync(Guid pagamentoId)
    {
        var pagamento = await ObterPagamentoPorIdAsync(pagamentoId)
            ?? throw new InvalidOperationException($"Pagamento com ID {pagamentoId} não encontrado");

        pagamento.Cancelar();
        _pagamentoRepository.Update(pagamento);
        await _pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        return pagamento;
    }

    public async Task<Pagamento?> ObterPagamentoPorIdAsync(Guid pagamentoId)
    {
        return await _pagamentoRepository.WithTracking()
                                       .GetAsync(pagamentoId);
    }

    public async Task<IEnumerable<Pagamento>> ObterPagamentosPorMatriculaIdAsync(Guid matriculaId)
    {
        return await _pagamentoRepository.WithTracking().GetAsync(p => p.MatriculaId == matriculaId)
            ?? Enumerable.Empty<Pagamento>();
    }

    public async Task<Pagamento> ProcessarPagamentoMatriculaAsync(Guid matriculaId, decimal valor, CartaoCredito cartaoCredito)
    {
        var matricula = await _estudanteRepository.GetMatriculaByIdAsync(matriculaId)
            ?? throw new InvalidOperationException($"Matrícula com ID {matriculaId} não encontrada");

        var pagamento = await CriarPagamentoAsync(matriculaId, valor);
        var idTransacao = Guid.CreateVersion7().ToString();
        pagamento = await ProcessarPagamentoAsync(pagamento.Id, idTransacao);

        PaymentBrokerResult result;
        try
        {
            result = await _paymentBrokerService.ProcessarPagamentoAsync(cartaoCredito);
        }
        catch (Exception e)
        {
            pagamento.MarcarComoFalha(e.Message);
            throw new DomainException(e.Message);
        }

        if (result.Success)
        {
            pagamento.ConfirmarPagamento(new CartaoCreditoData() { Hash = result.Hash });
        }
        else
        {
            pagamento.MarcarComoFalha(result.Details);
        }

        if (pagamento.Status == StatusPagamento.Pago)
        {
            matricula.ConfirmarPagamento();
            await _estudanteRepository.UnitOfWork.CommitAsync(CancellationToken.None);
        }

        await _pagamentoRepository.UnitOfWork.CommitAsync(CancellationToken.None);

        return pagamento;
    }
}