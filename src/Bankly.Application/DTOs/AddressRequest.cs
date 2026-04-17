using System;
using System.ComponentModel.DataAnnotations;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;

public record AddressRequest(
    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    Guid userId,

    [Required(ErrorMessage = "A rua é obrigatória.")]
    [StringLength(200, ErrorMessage = "A rua deve ter no máximo 200 caracteres.")]
    string street,

    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve ter exatamente 8 caracteres.")]
    string zipCode,

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    string city
)
{
    public Address ToDomain() => new Address(
        userId,
        street,
        zipCode,
        city
    );
}