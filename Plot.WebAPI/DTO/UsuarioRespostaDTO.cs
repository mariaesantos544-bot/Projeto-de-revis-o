using Plot.WebAPI.Models;

namespace Plot.WebAPI.DTO;

public class UsuarioRespostaDTO(int idUsuario, string Nome, string Email, string TipoUsuario, bool Ativo, string? FotoPerfil)
{
    public int IdUsuario { get; set; } = idUsuario;
    public string Nome { get; set; } = Nome;
    public string Email { get; set; } = Email;
    public string TipoUsuario { get; set; } = TipoUsuario;
    public bool Ativo { get; set; } = Ativo;
    public string? FotoPerfil { get; set; } = FotoPerfil;

    public static UsuarioRespostaDTO Converter(Usuario usuario) => new UsuarioRespostaDTO(
        usuario.IdUsuario,
        usuario.Nome,
        usuario.Email,
        usuario.TipoUsuario,
        usuario.Ativo,
        usuario.FotoPerfil
    );
}
