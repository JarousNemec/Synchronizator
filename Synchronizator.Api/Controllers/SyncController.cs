using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Synchronizator.Lib.Models;
using Synchronizator.Lib.Services;

namespace Synchronizator.Api.Controllers;

public class SyncController(DatabaseService databaseService, FileService fileService) : Controller
{
    private const string MIME_TYPE = "application/octet-stream";

    [HttpGet]
    public IActionResult Ping()
    {
        return Ok();
    }

    [HttpGet]
    public IActionResult GetRemoteState()
    {
        var token = Request.Headers["token"].First();
        if (string.IsNullOrEmpty(token))
            return Content("No in header");
        var userWithToken = databaseService.GetUserByToken(token);
        if (userWithToken == null)
            return BadRequest();
        var data = databaseService.GetUserFilesByToken(userWithToken.Id, token);
        return Json(data);
    }

    [HttpPost]
    public IActionResult Upload([FromForm] FileDataDto data)
    {
        if (fileService.SaveFile(data).Result.Completed)
            return Ok();
        return BadRequest();
    }

    public IActionResult Download()
    {
        var token = Request.Headers["token"];
        if (string.IsNullOrEmpty(token))
            return Content("No in header");
        var filename = Request.Headers["filename"].First();
        if (string.IsNullOrEmpty(filename))
            return Content("No filename in header");
        if (!Path.Exists(filename))
            return Content($"Bad filename cannot find file with path {filename}");
        if (!databaseService.IsFileAsociatedWithToken(filename, token))
            return Content($"Token isnt acosiated to this file so download is not permited");
        var filestream = GetFileStream(filename);
        return File(filestream, MIME_TYPE);
    }

    private Stream GetFileStream(string path)
    {
        Stream stream = Stream.Null;
        stream = System.IO.File.OpenRead(path);
        return stream;
    }
}