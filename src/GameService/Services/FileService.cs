


using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GameService.DTOs;

namespace GameService.Services;

public class FileService : IFileService
{
    private Cloudinary _cloudinary;
    private Account _account;

    private IConfiguration _configuration;

    public FileService(Cloudinary cloudinary, Account account, IConfiguration configuration)
    {
        _configuration = configuration;
        _account = new Account(_configuration.GetValue<string>("Cloudinary:cloudName"),
        _configuration.GetValue<string>("Cloudinary:apiKey"),
        _configuration.GetValue<string>("Cloudinary:apiSecret"));
        _configuration = configuration;

         _cloudinary = new Cloudinary(_account);
         _cloudinary.Api.Client.Timeout = TimeSpan.FromMinutes(30);
    }

    public Task UploadImage()
    {
        throw new NotImplementedException();
    }

    public async Task<string> UploadVideo(IFormFile File)
    {
        var uploadResult = new VideoUploadResult();
        if(File.Length > 0){
            using var stream = File.OpenReadStream();
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(File.FileName,stream),
                Folder="g-steam_microservices"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
            string videoUrl = _cloudinary.Api.UrlVideoUp.BuildUrl(uploadResult.PublicId);

            return videoUrl;

        }

        return "";
    }
}