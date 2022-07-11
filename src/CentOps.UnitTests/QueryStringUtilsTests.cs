using CentOps.Api.Controllers;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CentOps.UnitTests
{
    public class QueryStringUtilsTests
    {
        [Fact]
        public void GetParticipantTypeFilterThrowsForNoQueryCollection()
        {
            _ = Assert.Throws<ArgumentNullException>(() => QueryStringUtils.GetParticipantTypeFilter(null));
        }

        [Fact]
        public void GetParticipantTypeReturnsTheCorrectFilter()
        {
            var dictionary = new Dictionary<string, StringValues>
            {
                { "type", new StringValues("Dmr") }
            };

            var queryCollection = new QueryCollection(dictionary);
            var types = QueryStringUtils.GetParticipantTypeFilter(queryCollection);

            Assert.Equal(ParticipantTypeDto.Dmr, types.Single());
        }

        [Fact]
        public void GetParticipantTypeReturnsTheCorrectFilters()
        {
            var dictionary = new Dictionary<string, StringValues>
            {
                { "type", new StringValues(new string[] {"Dmr", "Classifier" }) }
            };

            var queryCollection = new QueryCollection(dictionary);
            var types = QueryStringUtils.GetParticipantTypeFilter(queryCollection);

            Assert.Equal(2, types.Count());
            _ = types.Should().BeEquivalentTo(new[] { ParticipantTypeDto.Dmr, ParticipantTypeDto.Classifier });
        }
    }
}
