using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
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
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(
        ITransactionRepository transactionRepository,
        ITransactionService transactionService,
        ILogger<TransactionController> logger)
    {
        _transactionRepository = transactionRepository;
        _transactionService = transactionService;
        _logger = logger;
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
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogInformation(
            "Iniciando criação de transação. TraceId: {TraceId}, AccountId: {AccountId}, Amount: {Amount}, Type: {Type}",
            traceId, request.accountId, request.amount, request.type);

        var transaction = _transactionService.Create(request);

        _logger.LogInformation(
            "Transação criada com sucesso. TraceId: {TraceId}, TransactionId: {TransactionId}",
            traceId, transaction.Id);

        return Ok(TransactionResponse.FromDomain(transaction));
    }
}