using Bankly.Application.Services;
using Bankly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bankly.Infrastructure.Repositories;

public class GenericRepository<T>(BanklyContext context) : IGenericRepository<T> where T : class
{
    protected readonly BanklyContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual T? GetById(Guid id) => _dbSet.Find(id);
    
    public virtual IEnumerable<T> GetAll() => _dbSet.ToList();
    
    public virtual void Add(T entity) 
    {
        _dbSet.Add(entity);
        _context.SaveChanges();
    }
    
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
        _context.SaveChanges();
    }
    
    public void SaveChanges() => _context.SaveChanges();
}