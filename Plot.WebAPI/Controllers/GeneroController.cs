using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plot.WebAPI.Interfaces;
using Plot.WebAPI.Models;

namespace Plot.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GeneroController : ControllerBase
{
    private readonly IGeneroRepository _generoRepository;

    public GeneroController(IGeneroRepository generoRepository)
    {
        _generoRepository = generoRepository;
    }

    /// <summary>
    /// Lista todos os gêneros em ordem de nome
    /// </summary>
    /// <returns>Lista de gêneros</returns>

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _generoRepository.Listar());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var genero = await _generoRepository.BuscarPorId(id);
        if genero is null ? return NotFound() : Ok(genero);
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] GeneroDTO dto)
    {
        var genero = new Genero { Nome = dto.Nome };
        await _generoRepository.Cadastrar(genero);
        return CreatedAtAction(nameof(BuscarPorId), new { id = genero.IdGenero }, genero);

    }
}
