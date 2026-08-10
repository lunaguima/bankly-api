namespace Bankly.Domain.Helpers;

public static class HashHelper
{
    /// <summary>
    /// Gera um hash seguro da senha. O BCrypt já gera e embute
    /// um salt aleatório dentro do hash retornado.
    /// </summary>
    public static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifica se a senha em texto puro corresponde ao hash armazenado.
    /// </summary>
    public static bool Verify(string rawPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(rawPassword, hashedPassword);
    }
}