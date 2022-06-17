using CentOps.Api.Controllers;

namespace CentOps.UnitTests
{
    public class ParticipantControllerTests
    {
        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new ParticipantController();
        }
    }
}
