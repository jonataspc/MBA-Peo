using Peo.Core.DomainObjects;
using Peo.Core.Entities.Base;

namespace Peo.StudentManagement.Domain.Entities;

public class Estudante : EntityBase, IAggregateRoot
{
    public Guid UserId { get; private set; }

    public bool IsActive { get; private set; }

    public virtual ICollection<Matricula> Matriculas { get; private set; } = [];

    public Estudante()
    {
    }

    public Estudante(Guid userId)
    {
        UserId = userId;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}