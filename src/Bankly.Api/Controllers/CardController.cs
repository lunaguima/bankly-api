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

    [HttpGet]
    public IActionResult GetAll()
    {
        var cards = _cardRepository.GetAll();
        var response = cards.Select(CardResponse.FromDomain).ToList();
        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CardRequest request) 
    {
        try
        {
            var entity = request.ToDomain();
            _cardRepository.Add(entity);
            
            return Ok(CardResponse.FromDomain(entity));
        }
        catch (InvalidOperationException ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] bool isActive)
    {
        var entity = _cardRepository.GetById(id);
        if (entity == null) 
            return NotFound();
        
        if(isActive)
            entity.ActivateCard();
        else
            entity.DeactivateCard();

        _cardRepository.Update(entity);
        
        return Ok(isActive ? "Cartão desbloqueado!" : "Cartão bloqueado com segurança!");
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var entity = _cardRepository.GetById(id);
        if (entity == null) 
            return NotFound();

        _cardRepository.Delete(entity);
        
        return NoContent();
    }
}