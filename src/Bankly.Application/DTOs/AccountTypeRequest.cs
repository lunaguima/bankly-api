using System.ComponentModel.DataAnnotations;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;


public record AccountTypeRequest(
    [Required(ErrorMessage = "O nome do tipo de conta é obrigatório!")]
    string name
)
{
    public AccountType ToDomain() => new AccountType(name);
}