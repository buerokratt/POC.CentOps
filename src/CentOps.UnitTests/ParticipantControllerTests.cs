using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CentOps.UnitTests
{
    public class ParticipantControllerTests
    {
        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new ParticipantController(new Mock<IParticipantStore>().Object, new Mock<IMapper>().Object);
        }

        [Fact]
        public async Task ReturnsAllParticipants()
        {
            // Arrange
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));

            var mockParticipantStore = new Mock<IParticipantStore>();
            var participants = new[]
            {
                new ParticipantDto { Id = "1", Name = "Test1", Status = ParticipantStatusDto.Active },
                new ParticipantDto { Id = "2", Name = "Test2", Status = ParticipantStatusDto.Disabled }
            };
            _ = mockParticipantStore.Setup(m => m.GetAll()).Returns(Task.FromResult(participants.AsEnumerable()));

            var sut = new ParticipantController(mockParticipantStore.Object, mapper.CreateMapper());

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(participants, okay.Value);
        }
    }
}
