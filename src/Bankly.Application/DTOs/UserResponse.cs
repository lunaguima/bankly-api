using System;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;


public record UserResponse(
    Guid id, 
    string name, 
    string cpf, 
    string email
)
{
 
    public static UserResponse FromDomain(User user) => new(
        user.Id, 
        user.Name, 
        user.Cpf, 
        user.Email
    );
}