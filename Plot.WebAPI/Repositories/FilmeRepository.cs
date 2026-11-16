using Microsoft.EntityFrameworkCore;
using Plot.WebAPI.BdContextEvent;
using Plot.WebAPI.Interfaces;
using Plot.WebAPI.Models;

namespace Plot.WebAPI.Repositories;

public class FilmeRepository : IFilmeRepository
{
    private readonly PlotDBContext _context;

    public FilmeRepository(PlotDBContext context)
    {
        _context = context;
    }

    public async Task Atualizar(int id, Filme filme)
    {
        var filmeExistente = await _context.Filme.Include(f => f.IdGenero).FirstOrDefaultAsync(f => f.IdFilme == id);
        if (filmeExistente != null)
        {
            filmeExistente.Titulo = filme.Titulo;
            filmeExistente.Sinopse = filme.Sinopse;
            filmeExistente.Duracao = filme.Duracao;
            filmeExistente.DataLancamento = filme.DataLancamento;
            filmeExistente.ImagemCapa = filme.ImagemCapa;

            var novosIds = filme.IdGenero.Select(g => g.IdGenero).ToHashSet();

            // 1. Remove apenas os gêneros que não estão mais presentes
            var generosParaRemover = filmeExistente.IdGenero.Where(g => !novosIds.Contains(g.IdGenero)).ToList();
            foreach (var genero in generosParaRemover)
            {
                filmeExistente.IdGenero.Remove(genero);
            }

            // 2. Adiciona novos gêneros que ainda não estavam associados
            var idsAtuais = filmeExistente.IdGenero.Select(g => g.IdGenero).ToHashSet();
            foreach (var genero in filme.IdGenero.Where(g => !idsAtuais.Contains(g.IdGenero)))
            {
                var trackedGenero = await _context.Genero.FindAsync(genero.IdGenero);
                if (trackedGenero != null)
                {
                    filmeExistente.IdGenero.Add(trackedGenero);
                }
                else
                {
                    _context.Attach(genero);
                    filmeExistente.IdGenero.Add(genero);
                }
            }

            await _context.SaveChangesAsync();
        }                           
    }

    public async Task<Filme?> BuscarPorId(int id)
    {
        return await _context.Filme.AsNoTracking().Include(f => f.IdGenero).FirstOrDefaultAsync(f => f.IdFilme == id);
    }

    public async Task Cadastrar(Filme filme)
    {
        await _context.Filme.AddAsync(filme);

        foreach (var genero in filme.IdGenero)
        {
            _context.Entry(genero).State = EntityState.Unchanged;
        }

        await _context.SaveChangesAsync();
    }

    public async Task Excluir(int id)
    {
        var filmeBuscado = await _context.Filme.Include(f => f.IdGenero).FirstOrDefaultAsync(f => f.IdFilme == id);

        if (filmeBuscado != null)
        {
            filmeBuscado.IdGenero.Clear();
            _context.Filme.Remove(filmeBuscado);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<Filme> filmes, int total)> ListarPaginado(int pagina = 1, int tamanhoPagina = 10, string? busca = null)
    {
        var query = _context.Filme.Include(f => f.IdGenero).AsQueryable();

        if (!string.IsNullOrEmpty(busca))
        {
            query = query.Where(f => f.Titulo.Contains(busca));
        }
        
        int total = await query.CountAsync();

        var items = await query
            .OrderBy(f => f.Titulo)
            .ThenBy(f => f.IdFilme)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (items, total);
    }
}
