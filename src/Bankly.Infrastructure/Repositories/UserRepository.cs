using Bankly.Application.Services;
using Bankly.Domain.Commom;
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

    public User Create(User user, Address address)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        if (address is null) throw new ArgumentNullException(nameof(address));

        if (ExistsByCpf(user.Cpf))
            throw new DomainException("Já existe um usuário com este CPF.");

        // Uma única transação: se qualquer SaveChanges falhar, nada é gravado.
        context.Users.Add(user);
        context.Addresses.Add(address);
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