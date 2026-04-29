using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smolla.IdentityProvider.Application.Abstractions;
using Smolla.IdentityProvider.Application.Clients;
using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Api.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize(Roles = Roles.Administrator)]
public sealed class ClientsController(IClientService clients) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClientResult>>> List(CancellationToken cancellationToken)
    {
        IReadOnlyList<ClientResult> results = await clients.ListAsync(cancellationToken);
        return Ok(results);
    }

    [HttpGet("{clientId}")]
    public async Task<ActionResult<ClientResult>> Get(string clientId, CancellationToken cancellationToken)
    {
        ClientResult? client = await clients.FindByClientIdAsync(clientId, cancellationToken);
        return client is null ? NotFound() : Ok(client);
    }
}
