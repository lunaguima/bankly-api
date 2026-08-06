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

    public TransactionController(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
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
    /// Registra uma nova transação (depósito, saque ou transferência) em uma conta.
    /// </summary>
    /// <param name="request">Dados da transação a ser registrada.</param>
    /// <returns>Transação criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TransactionRequest request)
    {
        var transaction = request.ToDomain();

        _transactionRepository.Add(transaction);

        return Ok(TransactionResponse.FromDomain(transaction));
    }
}