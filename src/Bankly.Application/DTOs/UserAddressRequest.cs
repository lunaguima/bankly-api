using System.ComponentModel.DataAnnotations;

namespace Bankly.Application.DTOs;

public record UserAddressRequest(
    [Required(ErrorMessage = "A rua é obrigatória.")]
    [StringLength(200, ErrorMessage = "A rua deve ter no máximo 200 caracteres.")]
    string street,

    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve ter exatamente 8 caracteres.")]
    string zipCode,

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    string city
);