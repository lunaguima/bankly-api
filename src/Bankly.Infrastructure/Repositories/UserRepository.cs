using Bankly.Application.Services;
using Bankly.Domain.Entities;
using Bankly.Infrastructure.Persistence;

namespace Bankly.Infrastructure.Repositories;

/// <summary>
/// Repositório para as operações de persistência e consulta dos usuários.
/// </summary>
public sealed class UserRepository(BanklyContext context) : IUserRepository
{
   
    public IReadOnlyList<User> GetAll() => context.Users.OrderBy(u => u.Name).ToList();

  
    public User? GetById(Guid id) => context.Users.FirstOrDefault(u => u.Id == id);

  
    public User Create(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        if (ExistsByCpf(user.Cpf))
            throw new InvalidOperationException("Já existe um usuário com este CPF.");

        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }
    
    public bool ExistsByCpf(string cpf) => context.Users.Any(u => u.Cpf == cpf);

 
    public void Update(User user)
    {
        context.Users.Update(user);
        context.SaveChanges();
    }
    
    public bool Delete(Guid id)
    {
        var user = context.Users.Find(id);
        if (user is null) return false;
        
        context.Users.Remove(user);
        context.SaveChanges();
        return true;
    }
}