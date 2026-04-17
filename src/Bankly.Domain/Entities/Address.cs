using System;
using Bankly.Domain.Commom;

namespace Bankly.Domain.Entities;

public class Address : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Street { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;

    // Relacionamento
    public User User { get; private set; }
    
    protected Address() { }

    public Address(Guid userId, string street, string zipCode, string city)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new Exception("A rua é obrigatória.");

        UserId = userId;
        Street = street;
        ZipCode = zipCode;
        City = city;
    }
    
    public void UpdateAddress(string newStreet, string newZipCode, string newCity)
    {
        if (string.IsNullOrWhiteSpace(newStreet)) throw new Exception("A rua é obrigatória.");
        
        Street = newStreet;
        ZipCode = newZipCode;
        City = newCity;
    }
}