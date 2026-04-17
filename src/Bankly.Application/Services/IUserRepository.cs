using Bankly.Application.DTOs;

namespace Bankly.Application.Services;
using Bankly.Domain.Entities;

public interface IUserRepository
{
    IReadOnlyList<User> GetAll(); 
    User? GetById(Guid id);
    User Create(User user);
    bool Delete(Guid id);
    bool ExistsByCpf(string cpf);
    void Update(User user);
    
}