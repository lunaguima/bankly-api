using System;
using System.ComponentModel.DataAnnotations;
using Bankly.Domain.Entities;
using Bankly.Domain.Enums;

namespace Bankly.Application.DTOs;

public record TransactionRequest(
    [Required(ErrorMessage = "A conta de origem é obrigatória.")]
    Guid accountId, 

    [Required(ErrorMessage = "O valor da transação é obrigatório.")]
    [Range(0.01, 1000000, ErrorMessage = "O valor deve ser entre 0.01 e 1.000.000.")]
    decimal amount, 

    [Required(ErrorMessage = "O tipo de transação é obrigatório.")]
    TransactionTypeEnum type 
)
{
    public Transaction ToDomain() => new Transaction(
        accountId,
        amount,
        type
    );
}