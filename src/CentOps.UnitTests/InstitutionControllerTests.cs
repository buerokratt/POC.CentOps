using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services;
using Microsoft.AspNetCore.Mvc;
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

        [Fact]
        public async Task ReturnsAllInsitutions()
        {
            // Arrange
            var mockInstitutions = new Mock<IInsitutionStore>();
            var institutions = new[]
            {
                new Institution { Id = "1", Name = "Test1", Status = InstitutionStatus.Active },
                new Institution { Id = "2", Name = "Test2", Status = InstitutionStatus.Disabled }
            };
            _ = mockInstitutions.Setup(m => m.GetAll()).Returns(Task.FromResult(institutions.AsEnumerable()));

            var sut = new InstitutionController(mockInstitutions.Object);

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(institutions, okay.Value);
        }
    }
}
