using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smolla.IdentityProvider.Application.Abstractions;
using Smolla.IdentityProvider.Application.Common;
using Smolla.IdentityProvider.Application.Users;
using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = Roles.Administrator)]
public sealed class UsersController(IUserService users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResult>>> Search(
        [FromQuery] string? search,
        [FromQuery] bool? includeDisabled,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        UserSearchQuery query = new()
        {
            Search = search,
            IncludeDisabled = includeDisabled,
            Skip = skip,
            Take = Math.Clamp(take, 1, 200)
        };

        IReadOnlyList<UserResult> results = await users.SearchAsync(query, cancellationToken);
        return Ok(results);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResult>> Get(Guid id, CancellationToken cancellationToken)
    {
        UserResult? user = await users.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResult>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        OperationResult<UserResult> result = await users.CreateAsync(request, cancellationToken);
        if (!result.Succeeded || result.Value is null)
        {
            return ValidationProblem(string.Join("; ", result.Errors));
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<UserResult>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        OperationResult<UserResult> result = await users.UpdateAsync(id, request, cancellationToken);
        return result.Succeeded && result.Value is not null
            ? Ok(result.Value)
            : ValidationProblem(string.Join("; ", result.Errors));
    }

    [HttpPost("{id:guid}/disable")]
    public async Task<ActionResult<UserResult>> Disable(Guid id, CancellationToken cancellationToken)
    {
        OperationResult<UserResult> result = await users.DisableAsync(id, cancellationToken);
        return result.Succeeded && result.Value is not null
            ? Ok(result.Value)
            : ValidationProblem(string.Join("; ", result.Errors));
    }

    [HttpPut("{id:guid}/roles")]
    public async Task<ActionResult<UserResult>> AssignRoles(Guid id, [FromBody] IReadOnlyCollection<string> roles, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roles);

        OperationResult<UserResult> result = await users.AssignRolesAsync(id, roles, cancellationToken);
        return result.Succeeded && result.Value is not null
            ? Ok(result.Value)
            : ValidationProblem(string.Join("; ", result.Errors));
    }
}
