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
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    public AccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var accountsEntity = _accountRepository.GetAll();
        var response = accountsEntity.Select(AccountResponse.FromDomain).ToList();
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var accountEntity = _accountRepository.GetById(id);
        
        if (accountEntity == null) 
            return NotFound();

        return Ok(AccountResponse.FromDomain(accountEntity));
    }

    [HttpPost]
    public IActionResult Create([FromBody] AccountRequest request)
    {
        try
        {
            var accountEntity = request.ToDomain(); 
            _accountRepository.Add(accountEntity); 
            
            return Ok(AccountResponse.FromDomain(accountEntity)); 
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro interno: " + ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] AccountRequest request)
    {
        try
        {
            var entity = _accountRepository.GetById(id);
            if (entity == null) 
                return NotFound();
            
            entity.UpdateDetails(request.branch, request.accountTypeId);

            _accountRepository.Update(entity); 

            return Ok("Conta atualizada com sucesso!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno ao atualizar conta: " + ex.Message);
        }
    }
    
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var accountEntity = _accountRepository.GetById(id);
        
        if (accountEntity == null) 
            return NotFound(); 

        _accountRepository.Delete(accountEntity); 
        return NoContent(); 
    }
}