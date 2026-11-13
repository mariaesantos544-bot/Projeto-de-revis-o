using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Plot.WebAPI.Utils;
using System.Text.RegularExpressions;

namespace Plot.WebAPI.Services;

public class CloudinaryService(
    IOptions<CloudinarySettings> options,
    ILogger<CloudinaryService> logger) : IImageService
{
    private readonly CloudinarySettings _settings = options.Value;

    private Cloudinary Cliente => new (new Account (_settings.CloudName, _settings.ApiKey, _settings.ApiSecret));
    public bool IsConfigured => 
        !string.IsNullOrWhiteSpace(_settings.CloudName)
        && !string.IsNullOrWhiteSpace(_settings.ApiKey)
        && !string.IsNullOrWhiteSpace(_settings.ApiSecret);

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (!IsConfigured || !Uri.TryCreate(imageUrl, UriKind.Absolute, out var url) || url is null || (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps) || !string.Equals(url.Host, "res.cloudinary.com", StringComparison.OrdinalIgnoreCase))
            return false;

        var match = Regex.Match(url.AbsolutePath, $@"^/(?i:{Regex.Escape(_settings.CloudName)})/image/upload/v[0-9]+/(plot/(?:filmes|usuarios)/[0-9a-fA-F]{{32}})\.[^/]+$");

        if (!match.Success) return false;

        var publicId = match.Groups[1].Value;

        try
        {
            return (await Cliente.DestroyAsync(new DeletionParams(publicId))).Result == "ok";
        }
        catch (Exception error)
        {
            logger.LogWarning(error, "Erro ao excluir imagem do Cloudinary: {PublicId}", publicId);
            return false;
        }
    }

    public async Task<string> UploadImageAsync(IFormFile arquivo, ImageFolder pasta)  
    {
        if(!IsConfigured)
            throw new InvalidOperationException("Cloudinary não está configurado corretamente.");
        if(arquivo is null || arquivo.Length is <= 0 or > 10 * 1024 * 1024)
            throw new ArgumentException("A imagem deve ter entre 1 byte e 10 MB.", 
            nameof(arquivo));
        if(Path.GetExtension(arquivo.FileName) is not (".jpg" or ".jpeg" or ".png"))
            throw new ArgumentException("A imagem deve ser do tipo JPG, JPEG ou PNG.", nameof(arquivo));
        
        using var dados = arquivo.OpenReadStream();

        var pastaCloud = pasta switch
        {
            ImageFolder.Filmes => "plot/filmes",
            ImageFolder.Usuarios => "plot/usuarios",
            _ => throw new ArgumentOutOfRangeException(nameof(pasta), pasta, null)
        };

        ImageUploadResult resultado;

        try
        {
            resultado = await Cliente.UploadAsync(new ImageUploadParams
            {
                File = new FileDescription(arquivo.FileName, dados),
                Folder = pastaCloud,
                PublicId = Guid.NewGuid().ToString("N"),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            });
        }
        catch (Exception error)
        {
            throw new HttpRequestException("Falha ao comunicar com o Cloudinary.", error);
        }

        return resultado.Error is null && resultado.SecureUrl is not null ? 
            resultado.SecureUrl.ToString() 
            : throw new HttpRequestException($"Falha ao enviar imagem para o Cloudinary: {resultado.Error?.Message}");
    }
}
