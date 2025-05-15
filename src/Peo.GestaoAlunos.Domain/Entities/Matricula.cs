using Peo.Core.Entities.Base;
using Peo.GestaoAlunos.Domain.ValueObjects;

namespace Peo.GestaoAlunos.Domain.Entities;

public class Matricula : EntityBase
{
    public Guid EstudanteId { get; private set; }
    public Guid CursoId { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public DateTime? DataConclusao { get; private set; }
    public StatusMatricula Status { get; private set; }
    public int PercentualProgresso { get; private set; }
    public virtual Estudante? Estudante { get; set; }

    protected Matricula()
    { }

    public Matricula(Guid estudanteId, Guid cursoId)
    {
        EstudanteId = estudanteId;
        CursoId = cursoId;
        DataMatricula = DateTime.Now;
        Status = StatusMatricula.PendentePagamento;
        PercentualProgresso = 0;
    }

    public void AtualizarProgresso(int percentual)
    {
        if (percentual < 0 || percentual > 100)
            throw new ArgumentException("O percentual de progresso deve estar entre 0 e 100");

        PercentualProgresso = percentual;
    }

    public void Concluir()
    {
        DataConclusao = DateTime.Now;
        Status = StatusMatricula.Concluido;
        PercentualProgresso = 100;
    }

    public void ConfirmarPagamento()
    {
        Status = StatusMatricula.Ativo;
    }

    public void Cancelar()
    {
        Status = StatusMatricula.Cancelado;
    }
}