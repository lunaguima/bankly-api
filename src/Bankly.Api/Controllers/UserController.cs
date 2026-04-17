using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
using Bankly.Domain.Entities;
using System;
using System.Linq;

namespace Bankly.Api.Controllers;

/// <summary>
/// Esse controller é responsável pelas operações de usuários
/// </summary>
/// <remarks>
/// URL: /api/user
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var usersEntity = _userRepository.GetAll();
        var response = usersEntity.Select(UserResponse.FromDomain).ToList();
        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] UserRequest request)
    {
        try
        {
            var userEntity = request.ToDomain(); 
            _userRepository.Create(userEntity); 

            return Ok(UserResponse.FromDomain(userEntity)); 
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
    public IActionResult Update(Guid id, [FromBody] UserRequest request)
    {
        try
        {
            var entity = _userRepository.GetById(id);
            if (entity == null) 
                return NotFound();
            
            entity.UpdateProfile(request.name, request.email, request.password);

            _userRepository.Update(entity);

            return Ok(UserResponse.FromDomain(entity));
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
            if (!_userRepository.Delete(id)) 
                return NotFound(); 

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno ao deletar: " + ex.Message);
        }
    }
}