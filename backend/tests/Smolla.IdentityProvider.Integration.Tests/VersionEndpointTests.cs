using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Smolla.IdentityProvider.Integration.Tests.Fixtures;

namespace Smolla.IdentityProvider.Integration.Tests;

public sealed class VersionEndpointTests : IClassFixture<IdentityProviderFactory>
{
    private readonly IdentityProviderFactory _factory;

    public VersionEndpointTests(IdentityProviderFactory factory)
    {
        _factory = factory;
    }

    private sealed record VersionResponse(string Version);

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GET_api_version_should_return_ok_with_a_version_string()
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(new Uri("/api/version", UriKind.Relative));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        VersionResponse? body = await response.Content.ReadFromJsonAsync<VersionResponse>();
        body.Should().NotBeNull();
        body!.Version.Should().NotBeNullOrWhiteSpace();
    }
}
