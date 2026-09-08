using Bankly.Domain.Entities;

namespace Bankly.Application.Services;

public interface IUserRepository
{
    IReadOnlyList<User> GetAll();
    User? GetById(Guid id);
    User Create(User user, Address address);
    bool Delete(Guid id);
    bool ExistsByCpf(string cpf);
    void Update(User user);
}