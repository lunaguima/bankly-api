using Microsoft.AspNetCore.Mvc;
using Bankly.Application.DTOs;
using Bankly.Application.Services;
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

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    /// <returns>Lista de usuários.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var usersEntity = _userRepository.GetAll();
        var response = usersEntity.Select(UserResponse.FromDomain).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Cria um novo usuário junto com o endereço informado.
    /// </summary>
    /// <param name="request">Dados do usuário e do endereço a serem criados.</param>
    /// <returns>Usuário criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] UserRequest request)
    {
        var userEntity = request.ToDomain();
        var addressEntity = request.ToAddressDomain(userEntity.Id);

        _userRepository.Create(userEntity, addressEntity);

        return Ok(UserResponse.FromDomain(userEntity));
    }

    /// <summary>
    /// Atualiza os dados de perfil de um usuário existente.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="request">Novos dados do usuário.</param>
    /// <returns>Usuário atualizado.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] UserRequest request)
    {
        var entity = _userRepository.GetById(id);
        if (entity == null)
            return NotFound();

        entity.UpdateProfile(request.name, request.email, request.password);

        _userRepository.Update(entity);

        return Ok(UserResponse.FromDomain(entity));
    }

    /// <summary>
    /// Remove um usuário existente.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!_userRepository.Delete(id))
            return NotFound();

        return NoContent();
    }
}