using CentOps.Api.Authentication.Models;
using FluentAssertions;
using System.Security.Claims;

namespace CentOps.UnitTests.Authentication.Models
{
    public class ApiUserTests
    {
        [Fact]
        public void CtorShouldSetClaimsProperty()
        {
            var user = new ApiUser(new[]
            {
                new Claim("type1", "value1"),
                new Claim("type2", "value2")
            });

            _ = user.Claims.Should().HaveCount(2);

            var claim1 = user.Claims.ElementAt(0);
            _ = claim1.Type.Should().Be("type1");
            _ = claim1.Value.Should().Be("value1");

            var claim2 = user.Claims.ElementAt(1);
            _ = claim2.Type.Should().Be("type2");
            _ = claim2.Value.Should().Be("value2");
        }
    }
}
