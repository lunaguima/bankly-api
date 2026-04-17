using System;
using System.ComponentModel.DataAnnotations;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;


public record AccountRequest(
    [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
    Guid userId,

    [Required(ErrorMessage = "O tipo de conta é obrigatório.")]
    Guid accountTypeId, 

    [Required(ErrorMessage = "A agência é obrigatória.")]
    string branch, 

    [Required(ErrorMessage = "O número da conta é obrigatório.")]
    string accountNumber, 

    [Range(0, double.MaxValue, ErrorMessage = "O saldo inicial não pode ser negativo.")]
    decimal initialBalance
)
{
    public Account ToDomain() => new Account(
        userId,
        accountTypeId,
        branch,          
        accountNumber, 
        initialBalance
    );
}