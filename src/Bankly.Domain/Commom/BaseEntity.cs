using System;

namespace Bankly.Domain.Commom;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public bool Active { get; protected set; } = true;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}