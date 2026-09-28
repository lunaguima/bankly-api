using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
using System;
using System.Linq;

namespace Bankly.Api.Controllers;

/// <summary>
/// Esse controller é responsável pelas operações de tipos de conta na API.
/// Possui os endpoints para criar, listar, atualizar e remover tipos de conta.
/// </summary>
/// <remarks>
/// URL: /api/accounttype
/// </remarks>
[Route("api/[controller]")]
[ApiVersionNeutral]
[ApiController]
public class AccountTypeController : ControllerBase
{
    private readonly IAccountTypeRepository _accountTypeRepository;

    public AccountTypeController(IAccountTypeRepository accountTypeRepository)
    {
        _accountTypeRepository = accountTypeRepository;
    }

    /// <summary>
    /// Lista todos os tipos de conta cadastrados.
    /// </summary>
    /// <returns>Lista de tipos de conta.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountTypeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var entities = _accountTypeRepository.GetAll();
        var response = entities.Select(AccountTypeResponse.FromDomain).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Cria um novo tipo de conta.
    /// </summary>
    /// <param name="request">Nome do tipo de conta.</param>
    /// <returns>Tipo de conta criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AccountTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AccountTypeRequest request)
    {
        var entity = request.ToDomain();
        _accountTypeRepository.Add(entity);

        return Ok(AccountTypeResponse.FromDomain(entity));
    }

    /// <summary>
    /// Atualiza o nome de um tipo de conta existente.
    /// </summary>
    /// <param name="id">Identificador do tipo de conta.</param>
    /// <param name="request">Novo nome do tipo de conta.</param>
    /// <returns>Tipo de conta atualizado.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] AccountTypeRequest request)
    {
        var entity = _accountTypeRepository.GetById(id);
        if (entity == null)
            return NotFound();

        entity.UpdateName(request.name);

        _accountTypeRepository.Update(entity);

        return Ok(AccountTypeResponse.FromDomain(entity));
    }

    /// <summary>
    /// Remove um tipo de conta existente.
    /// </summary>
    /// <param name="id">Identificador do tipo de conta.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var entity = _accountTypeRepository.GetById(id);
        if (entity == null)
            return NotFound();

        _accountTypeRepository.Delete(entity);

        return NoContent();
    }
}