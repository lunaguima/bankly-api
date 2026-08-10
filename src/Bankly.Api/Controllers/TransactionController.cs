using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
using Bankly.Domain.Entities;
using System;
using System.Linq;

namespace Bankly.Api.Controllers;

/// <summary>
/// Controller responsável pelas operações de transações na API.
/// Ele possui os endpoints de criar e listar as transações.
/// </summary>
/// <remarks>
/// URL: /api/transaction
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    public TransactionController(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    /// <summary>
    /// Lista todas as transações cadastradas.
    /// </summary>
    /// <returns>Lista de transações.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var entities = _transactionRepository.GetAll();
        var response = entities.Select(TransactionResponse.FromDomain).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Registra uma nova transação (depósito, saque ou transferência) em uma conta,
    /// atualizando o saldo correspondente.
    /// </summary>
    /// <param name="request">Dados da transação a ser registrada.</param>
    /// <returns>Transação criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TransactionRequest request)
    {
        var account = _accountRepository.GetById(request.accountId);
        if (account == null)
            return NotFound();

        var transaction = request.ToDomain();

        account.ApplyTransaction(transaction);
        _accountRepository.Update(account);
        _transactionRepository.Add(transaction);

        return Ok(TransactionResponse.FromDomain(transaction));
    }
}