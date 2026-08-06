using System.ComponentModel.DataAnnotations;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;

public record UserRequest(
    [Required(ErrorMessage = "O campo nome é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
    string name,

    [Required(ErrorMessage = "O campo CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 11 números.")]
    string cpf,

    [Required(ErrorMessage = "O campo email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
    string email,

    [Required(ErrorMessage = "O campo senha é obrigatório.")]
    [StringLength(30, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    string password,

    [Required(ErrorMessage = "O endereço é obrigatório.")]
    UserAddressRequest address
)
{
    public User ToDomain() => new User(name, cpf, email, password);

    /// <summary>
    /// Cria a entidade Address vinculada ao usuário. Deve ser chamado
    /// depois que o User já foi instanciado (o Id é gerado no construtor).
    /// </summary>
    public Address ToAddressDomain(Guid userId) => new Address(
        userId,
        address.street,
        address.zipCode,
        address.city
    );
}