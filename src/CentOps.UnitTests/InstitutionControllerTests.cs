using CentOps.Api.Controllers;

namespace CentOps.UnitTests
{
    public class InstitutionControllerTests
    {
        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new InstitutionController();
        }
    }
}
