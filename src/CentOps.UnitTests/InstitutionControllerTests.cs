using CentOps.Api.Controllers;
using CentOps.Api.Services;
using Moq;

namespace CentOps.UnitTests
{
    public class InstitutionControllerTests
    {
        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new InstitutionController(new Mock<IInsitutionStore>().Object);
        }
    }
}
