namespace Plot.WebAPI.Utils;

public class Criptografia
{
    public static string gerarHash(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public static string GerarHash(string senha) => gerarHash(senha);

    public static bool compararHash(string senhaInformada, string senhaBanco)
    {
        if (string.IsNullOrEmpty(senhaInformada) || string.IsNullOrEmpty(senhaBanco))
        {
            return false;
        }

        if (!senhaBanco.StartsWith("$2", StringComparison.Ordinal))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(senhaInformada, senhaBanco);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }

    public static bool CompararHash(string senhaInformada, string senhaBanco) => compararHash(senhaInformada, senhaBanco);
}
