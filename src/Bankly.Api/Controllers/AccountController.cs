using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
using System;
using System.Linq;

namespace Bankly.Api.Controllers;

/// <summary>
/// Controller responsável pelas operações de contas bancárias na API.
/// Expõe endpoints para criar, listar, buscar, atualizar e remover contas.
/// </summary>
[Route("api/[controller]")]
[ApiVersionNeutral]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    public AccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    /// <summary>
    /// Lista todas as contas cadastradas.
    /// </summary>
    /// <returns>Lista de contas.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var accountsEntity = _accountRepository.GetAll();
        var response = accountsEntity.Select(AccountResponse.FromDomain).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Busca uma conta específica pelo Id.
    /// </summary>
    /// <param name="id">Identificador da conta.</param>
    /// <returns>Dados da conta encontrada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var accountEntity = _accountRepository.GetById(id);

        if (accountEntity == null)
            return NotFound();

        return Ok(AccountResponse.FromDomain(accountEntity));
    }

    /// <summary>
    /// Cria uma nova conta bancária.
    /// </summary>
    /// <param name="request">Dados da conta a ser criada.</param>
    /// <returns>Conta criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AccountRequest request)
    {
        var accountEntity = request.ToDomain();
        _accountRepository.Add(accountEntity);

        return Ok(AccountResponse.FromDomain(accountEntity));
    }

    /// <summary>
    /// Atualiza a agência e o tipo de conta de uma conta existente.
    /// </summary>
    /// <param name="id">Identificador da conta.</param>
    /// <param name="request">Novos dados da conta.</param>
    /// <returns>Conta atualizada.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] AccountRequest request)
    {
        var entity = _accountRepository.GetById(id);
        if (entity == null)
            return NotFound();

        entity.UpdateDetails(request.branch, request.accountTypeId);

        _accountRepository.Update(entity);

        return Ok(AccountResponse.FromDomain(entity));
    }

    /// <summary>
    /// Remove uma conta existente.
    /// </summary>
    /// <param name="id">Identificador da conta.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var accountEntity = _accountRepository.GetById(id);

        if (accountEntity == null)
            return NotFound();

        _accountRepository.Delete(accountEntity);
        return NoContent();
    }
}