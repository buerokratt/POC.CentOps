using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services;
using Microsoft.AspNetCore.Mvc;
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

        [Fact]
        public async Task ReturnsAllParticipants()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            var participants = new[]
            {
                new Participant { Id = "1", Name = "Test1", Status = ParticipantStatus.Active },
                new Participant { Id = "2", Name = "Test2", Status = ParticipantStatus.Disabled }
            };
            _ = mockParticipantStore.Setup(m => m.GetAll()).Returns(Task.FromResult(participants.AsEnumerable()));

            var sut = new ParticipantController(mockParticipantStore.Object);

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(participants, okay.Value);
        }
    }
}
