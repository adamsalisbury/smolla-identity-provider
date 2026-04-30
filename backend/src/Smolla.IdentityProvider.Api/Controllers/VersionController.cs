using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Smolla.IdentityProvider.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VersionController : ControllerBase
{
    private static readonly string AssemblyVersion =
        typeof(VersionController).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(VersionController).Assembly.GetName().Version?.ToString(3)
        ?? "unknown";

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { version = AssemblyVersion });
    }
}
