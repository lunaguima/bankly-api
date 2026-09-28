using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
using System;
using System.Linq;

namespace Bankly.Api.Controllers;

/// <summary>
/// Controller responsável pelas operações de transações na API.
/// Possui os endpoints de criar e listar as transações.
/// A listagem existe em duas versões: 1.0 (deprecada, devolve lista) e 2.0 (paginada).
/// </summary>
/// <remarks>
/// URL: /api/transaction
/// </remarks>
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(
        ITransactionService transactionService,
        ILogger<TransactionController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// [DEPRECADO] Lista todas as transações cadastradas (contrato antigo, sem paginação).
    /// </summary>
    /// <returns>Lista de transações.</returns>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAllV1()
    {
        var entities = _transactionService.GetAll();
        var response = entities.Select(TransactionResponse.FromDomain).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista as transações de forma paginada.
    /// </summary>
    /// <param name="page">Número da página (mínimo 1). Padrão: 1.</param>
    /// <param name="pageSize">Itens por página (1 a 100). Padrão: 20.</param>
    /// <returns>Envelope com a página, os totais e os itens.</returns>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(PagedResponse<TransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult GetPagedV2([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = _transactionService.GetPaged(page, pageSize);
        return Ok(response);
    }

    /// <summary>
    /// Registra uma nova transação (depósito, saque ou transferência) em uma conta,
    /// atualizando o saldo correspondente. Limitado a 10 requisições por minuto.
    /// </summary>
    /// <param name="request">Dados da transação a ser registrada.</param>
    /// <returns>Transação criada.</returns>
    [HttpPost]
    [EnableRateLimiting("escrita")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
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