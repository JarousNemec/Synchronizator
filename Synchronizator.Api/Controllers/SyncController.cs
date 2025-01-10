using Microsoft.AspNetCore.Mvc;

namespace Synchronizator.Api.Controllers;

public class SyncController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }
}