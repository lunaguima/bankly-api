using Asp.Versioning;
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
[ApiVersionNeutral]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IAddressRepository _addressRepository;

    public AddressController(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    /// <summary>
    /// Lista todos os endereços cadastrados.
    /// </summary>
    /// <returns>Lista de endereços.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AddressResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var entities = _addressRepository.GetAll();
        var response = entities.Select(AddressResponse.FromDomain).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Cria um novo endereço para um usuário existente.
    /// </summary>
    /// <param name="request">Dados do endereço a ser criado.</param>
    /// <returns>Endereço criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AddressRequest request)
    {
        var entity = request.ToDomain();
        _addressRepository.Add(entity);
        return Ok(AddressResponse.FromDomain(entity));
    }

    /// <summary>
    /// Atualiza os dados de um endereço existente.
    /// </summary>
    /// <param name="id">Identificador do endereço.</param>
    /// <param name="request">Novos dados do endereço.</param>
    /// <returns>Endereço atualizado.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] AddressRequest request)
    {
        var entity = _addressRepository.GetById(id);
        if (entity == null)
            return NotFound();

        entity.UpdateAddress(request.street, request.zipCode, request.city);

        _addressRepository.Update(entity);
        return Ok(AddressResponse.FromDomain(entity));
    }

    /// <summary>
    /// Remove um endereço existente.
    /// </summary>
    /// <param name="id">Identificador do endereço.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var entity = _addressRepository.GetById(id);
        if (entity == null)
            return NotFound();

        _addressRepository.Delete(entity);
        return NoContent();
    }
}