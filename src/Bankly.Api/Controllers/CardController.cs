using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
using System;
using System.Linq;

namespace Bankly.Api.Controllers;

/// <summary>
/// Controller responsável pelas operações de cartões na API.
/// Expõe endpoints para criar, listar, buscar, atualizar status e remover cartões.
/// </summary>
/// <remarks>
/// URL: /api/card
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class CardController : ControllerBase
{
    private readonly ICardRepository _cardRepository;

    public CardController(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    /// <summary>
    /// Lista todos os cartões cadastrados.
    /// </summary>
    /// <returns>Lista de cartões.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CardResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var cards = _cardRepository.GetAll();
        var response = cards.Select(CardResponse.FromDomain).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Cria um novo cartão vinculado a uma conta existente.
    /// </summary>
    /// <param name="request">Dados do cartão a ser criado.</param>
    /// <returns>Cartão criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CardRequest request)
    {
        var entity = request.ToDomain();
        _cardRepository.Add(entity);

        return Ok(CardResponse.FromDomain(entity));
    }

    /// <summary>
    /// Ativa ou bloqueia um cartão existente.
    /// </summary>
    /// <param name="id">Identificador do cartão.</param>
    /// <param name="isActive">true para ativar, false para bloquear.</param>
    /// <returns>Mensagem de confirmação.</returns>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateStatus(Guid id, [FromBody] bool isActive)
    {
        var entity = _cardRepository.GetById(id);
        if (entity == null)
            return NotFound();

        if (isActive)
            entity.ActivateCard();
        else
            entity.DeactivateCard();

        _cardRepository.Update(entity);

        return Ok(isActive ? "Cartão desbloqueado!" : "Cartão bloqueado com segurança!");
    }

    /// <summary>
    /// Remove um cartão existente.
    /// </summary>
    /// <param name="id">Identificador do cartão.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var entity = _cardRepository.GetById(id);
        if (entity == null)
            return NotFound();

        _cardRepository.Delete(entity);

        return NoContent();
    }
}