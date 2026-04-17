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

    [HttpGet]
    public IActionResult GetAll()
    {
        var entities = _transactionRepository.GetAll();
        var response = entities.Select(TransactionResponse.FromDomain).ToList();
        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] TransactionRequest request)
    {
        try
        {
            var transaction = request.ToDomain();

            _transactionRepository.Add(transaction);
            
            return Ok(TransactionResponse.FromDomain(transaction));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            var erroReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return StatusCode(500, $"Erro: {erroReal}");
        }
    }
}