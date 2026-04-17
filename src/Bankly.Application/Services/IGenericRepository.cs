namespace Bankly.Application.Services;

public interface IGenericRepository<T> where T : class
{
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SaveChanges();
}