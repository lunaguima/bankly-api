using System;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;


public record AddressResponse(
    Guid id,
    Guid userId,
    string street,
    string zipCode,
    string city
)
{
   
    public static AddressResponse FromDomain(Address a) => new(
        a.Id, 
        a.UserId, 
        a.Street, 
        a.ZipCode, 
        a.City
    );
}