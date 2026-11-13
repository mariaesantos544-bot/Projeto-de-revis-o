using Plot.WebAPI.Interfaces;
using Plot.WebAPI.Models;

namespace Plot.WebAPI.Repositories
{
    public class FilmeRepository : IFilmeRepository
    {
        public Task Atualizar(int id, Filme filme)
        {
            throw new NotImplementedException();
        }

        public Task<Filme?> BuscarPorId(int id)
        {
            throw new NotImplementedException();
        }

        public Task Cadastrar(Filme filme)
        {
            throw new NotImplementedException();
        }

        public Task Excluir(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(List<Filme> filmes, int total)> ListarPaginado(int pagina = 1, int tamanhoPagina = 10, string? busca = null)
        {
            throw new NotImplementedException();
        }
    }
}
