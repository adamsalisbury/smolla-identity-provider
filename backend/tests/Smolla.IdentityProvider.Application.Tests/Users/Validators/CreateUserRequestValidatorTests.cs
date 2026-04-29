using FluentAssertions;
using FluentValidation.Results;
using Smolla.IdentityProvider.Application.Users;
using Smolla.IdentityProvider.Application.Users.Validators;

namespace Smolla.IdentityProvider.Application.Tests.Users.Validators;

public sealed class CreateUserRequestValidatorTests
{
    private readonly CreateUserRequestValidator _validator = new();

    [Fact]
    [Trait("Category", "Unit")]
    public void Valid_request_should_pass()
    {
        CreateUserRequest request = new()
        {
            Email = "user@example.com",
            Password = "Password123",
            DisplayName = "Sample User"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Invalid_email_should_fail(string email)
    {
        CreateUserRequest request = new()
        {
            Email = email,
            Password = "Password123",
            DisplayName = "Sample User"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserRequest.Email));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Short_password_should_fail()
    {
        CreateUserRequest request = new()
        {
            Email = "user@example.com",
            Password = "short",
            DisplayName = "Sample User"
        };

        ValidationResult result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserRequest.Password));
    }
}
