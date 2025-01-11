using Microsoft.AspNetCore.Http;

namespace Synchronizator.Lib.Models;

public class FileDataDto
{
    public IFormFile File { get; set; }
    public FileDetailsDto Details { get; set; }
}