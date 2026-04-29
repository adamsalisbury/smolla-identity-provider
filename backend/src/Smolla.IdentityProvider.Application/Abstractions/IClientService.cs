using Smolla.IdentityProvider.Application.Clients;

namespace Smolla.IdentityProvider.Application.Abstractions;

/// <summary>
/// High-level OAuth client management operations consumed by API controllers
/// and the seeder.
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Returns every registered OAuth client.
    /// </summary>
    Task<IReadOnlyList<ClientResult>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a registered client by its public <c>client_id</c> value.
    /// </summary>
    Task<ClientResult?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
}
