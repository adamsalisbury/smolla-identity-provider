using System.Net;
using FluentAssertions;
using Smolla.IdentityProvider.Integration.Tests.Fixtures;

namespace Smolla.IdentityProvider.Integration.Tests;

public sealed class HealthEndpointTests : IClassFixture<IdentityProviderFactory>
{
    private readonly IdentityProviderFactory _factory;

    public HealthEndpointTests(IdentityProviderFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GET_api_health_should_return_ok()
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(new Uri("/api/health", UriKind.Relative));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
