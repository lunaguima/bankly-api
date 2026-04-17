using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
using Bankly.Domain.Entities;
using System;
using System.Linq;

namespace Bankly.Api.Controllers;

/// <summary>
/// Esse controller é responsável pelas operações de endereços na API.
/// Possui os endpoints para criar, listar, atualizar e remover endereços.
/// </summary>
/// <remarks>
/// URL: /api/address
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IAddressRepository _addressRepository;

    public AddressController(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var entities = _addressRepository.GetAll();
        var response = entities.Select(AddressResponse.FromDomain).ToList();
        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] AddressRequest request)
    {
        try
        {
            var entity = request.ToDomain(); 
            _addressRepository.Add(entity);
            return Ok(AddressResponse.FromDomain(entity));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro ao criar endereço: " + ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] AddressRequest request)
    {
        var entity = _addressRepository.GetById(id);
        if (entity == null) 
            return NotFound();
        
        entity.UpdateAddress(request.street, request.zipCode, request.city);

        _addressRepository.Update(entity);
        return Ok(AddressResponse.FromDomain(entity));
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var entity = _addressRepository.GetById(id);
        if (entity == null) 
            return NotFound();

        _addressRepository.Delete(entity);
        return NoContent();
    }
}