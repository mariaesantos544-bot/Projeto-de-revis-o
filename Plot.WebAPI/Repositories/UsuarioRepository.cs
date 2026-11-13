using Microsoft.EntityFrameworkCore;
using Plot.WebAPI.BdContextEvent;
using Plot.WebAPI.Interfaces;
using Plot.WebAPI.Models;
using Plot.WebAPI.Utils;

namespace Plot.WebAPI.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly PlotDBContext _context;

    public UsuarioRepository(PlotDBContext context)
    {
        _context = context;
    }

    public async Task Atualizar(int id, Usuario usuario)
    {
        var usuarioExistente = await _context.Usuario.FindAsync(id);
        if (usuarioExistente != null)
        {
            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Email = usuario.Email;
            usuarioExistente.TipoUsuario = usuario.TipoUsuario;
            usuarioExistente.Ativo = usuario.Ativo;
            usuarioExistente.FotoPerfil = usuario.FotoPerfil;

            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuarioExistente.Senha = Criptografia.gerarHash(usuario.Senha);
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Usuario?> BuscarPorEmailESenha(string email, string senha)
    {
        var usuarioBuscado = await _context.Usuario.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

        if (usuarioBuscado == null)
        {
            return null;
        }

        bool senhaValida = Criptografia.compararHash(senha, usuarioBuscado.Senha);

        if (!senhaValida)
        {
            return null;
        }

        return usuarioBuscado;
    }

    public async Task<Usuario?> BuscarPorId(int id)
    {
        return await _context.Usuario.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);
    }

    public async Task Cadastrar(Usuario usuario)
    {
        usuario.Senha = Criptografia.gerarHash(usuario.Senha);

        await _context.Usuario.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task Excluir(int id)
    {
        var usuarioBuscado = await _context.Usuario.FindAsync(id);

        if (usuarioBuscado != null)
        {
            _context.Usuario.Remove(usuarioBuscado);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Usuario>> Listar()
    {
        return await _context.Usuario.AsNoTracking().ToListAsync();
    }
}
