using Domain.Users;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Users;

public sealed class UserTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var id = Guid.NewGuid();
        var user = User.Create(id, "a@b.com", UserRole.Patient.ToString());

        user.Id.Should().Be(id);
        user.Email.Should().Be("a@b.com");
        user.Role.Should().Be(UserRole.Patient.ToString());
    }
}