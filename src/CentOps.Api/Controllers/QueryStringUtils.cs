using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Controllers
{
    public static class QueryStringUtils
    {
        public static IEnumerable<ParticipantTypeDto> GetParticipantTypeFilter(IQueryCollection query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (query.ContainsKey("type"))
            {
                var types = query["type"];

                var correctTypes = types.Select(s =>
                {
                    return Enum.TryParse<ParticipantTypeDto>(s, out var type) ? type : ParticipantTypeDto.Unknown;
                }).Where(t => t != ParticipantTypeDto.Unknown);

                return correctTypes;
            }

            return Enumerable.Empty<ParticipantTypeDto>();
        }
    }
}
