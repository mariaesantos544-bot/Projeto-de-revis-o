
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plot.WebAPI.DTO;
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
        return genero is null ? NotFound() : Ok(genero);
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] GeneroDTO dto)
    {
        var genero = new Genero { Nome = dto.Nome };

        try
        {
            await _generoRepository.Cadastrar(genero)
        }
       
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] GeneroDTO dto)
    {
        if (await _generoRepository.BuscarPorId(id) is null) return NotFound();
        await _generoRepository.Atualizar(id, new Genero { Nome = dto.Nome });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    { 
        if (await _generoRepository.BuscarPorId(id) is null) return NotFound();
        await _generoRepository.Excluir(id);
        return NoContent();
    }
}
