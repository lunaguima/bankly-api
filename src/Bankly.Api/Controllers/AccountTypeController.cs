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
[ApiController]
public class AccountTypeController : ControllerBase
{
    private readonly IAccountTypeRepository _accountTypeRepository;

    public AccountTypeController(IAccountTypeRepository accountTypeRepository)
    {
        _accountTypeRepository = accountTypeRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var entities = _accountTypeRepository.GetAll();
        var response = entities.Select(AccountTypeResponse.FromDomain).ToList();
        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] AccountTypeRequest request)
    {
        try
        {
            var entity = request.ToDomain();
            _accountTypeRepository.Add(entity); 
            
            return Ok(AccountTypeResponse.FromDomain(entity)); 
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno: " + ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] AccountTypeRequest request)
    {
        try
        {
            var entity = _accountTypeRepository.GetById(id);
            if (entity == null) 
                return NotFound();
            
            entity.UpdateName(request.name);

            _accountTypeRepository.Update(entity); 

            return Ok(AccountTypeResponse.FromDomain(entity));
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno ao atualizar: " + ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            var entity = _accountTypeRepository.GetById(id);
            if (entity == null) 
                return NotFound();

            _accountTypeRepository.Delete(entity); 

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno ao deletar: " + ex.Message);
        }
    }
}