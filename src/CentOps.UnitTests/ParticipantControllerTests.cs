using CentOps.Api.Controllers;
using CentOps.Api.Services;
using Moq;

namespace CentOps.UnitTests
{
    public class ParticipantControllerTests
    {
        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new ParticipantController(new Mock<IParticipantStore>().Object);
        }
    }
}
