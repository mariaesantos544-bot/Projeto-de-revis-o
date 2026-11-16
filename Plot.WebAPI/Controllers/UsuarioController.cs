
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plot.WebAPI.DTO;
using Plot.WebAPI.Interfaces;
using Plot.WebAPI.Models;
using Plot.WebAPI.Services;
using System.Security.Claims;

namespace Plot.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IImageService _imageService;

        public UsuarioController(IUsuarioRepository usuarioRepository, IImageService imageService)
        {
            _usuarioRepository = usuarioRepository;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _usuarioRepository.Listar();
            return Ok(usuarios.Select(UsuarioRespostaDTO.Converter));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            if (!PodeAcessar(id)) return Forbid();

            var usuario = await _usuarioRepository.BuscarPorId(id);
            return usuario is null ? NotFound() : Ok(UsuarioRespostaDTO.Converter(usuario));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(11 * 1024 * 1024)]
        public async Task<IActionResult> Cadastrar([FromForm] UsuarioDTO dto)
        {
            if (dto.ArquivoImagem is not null && !_imageService.IsConfigured)
                return Problem("Configure as credenciais do Cloudinary para enviar imagens.", statusCode: 503);

            string? novaUrl = null;
            try
            {
                if (dto.ArquivoImagem is not null)
                    novaUrl = await _imageService.UploadImageAsync(dto.ArquivoImagem, ImageFolder.Usuarios);
            }
            catch (ArgumentException erro) { return BadRequest(erro.Message); }
            catch (HttpRequestException) { return Problem("Falha ao enviar foto ao Cloudinary.", statusCode: 502); }

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                TipoUsuario = dto."Usuario",
                Ativo = true,
                FotoPerfil = novaUrl ?? (string.IsNullOrWhiteSpace(dto.FotoPerfil) ? null : dto.FotoPerfil)
            };

            try
            {
                await _usuarioRepository.Cadastrar(usuario);
            }
            catch
            {
                if (novaUrl is not null) await _imageService.DeleteImageAsync(novaUrl);
                throw;
            }

            return CreatedAtAction(nameof(BuscarPorId), new { id = usuario.IdUsuario }, UsuarioRespostaDTO.Converter(usuario));
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(11 * 1024 * 1024)]

        public async Task<IActionResult> Atualizar(int id, [FromForm] UsuarioAtualizacaoDTO dto)
        {
            if (!PodeAcessar(id)) return Forbid();
            var atual = await _usuarioRepositoryg

        }

        private bool PodeAcessar(int id) => User.IsInRole("Admin") || (int.TryParse(User.FindFirstValue(JwtBearerExtensions.Sub), out var usuarioId) && usuarioId == id);
    }
}
