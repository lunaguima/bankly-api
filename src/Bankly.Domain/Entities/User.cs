using System;
using System.Collections.Generic;
using Bankly.Domain.Commom;

namespace Bankly.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;

    // Relacionamentos
    public Address Address { get; private set; }
    public List<Account> Accounts { get; private set; } = [];

    protected User() { }

    public User(string name, string cpf, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(cpf)) throw new DomainException("CPF é obrigatório.");

        Name = name;
        Cpf = cpf;
        Email = email;
        Password = password;
    }

    public void UpdateProfile(string newName, string newEmail, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new DomainException("O nome não pode ser vazio.");
        Name = newName;
        Email = newEmail;
        Password = newPassword;
    }
}