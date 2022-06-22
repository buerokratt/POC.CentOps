using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace CentOps.UnitTests
{
    public class ParticipantControllerTests
    {
        private readonly MapperConfiguration _mapper;

        public ParticipantControllerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
        }

        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new ParticipantController(new Mock<IParticipantStore>().Object, new Mock<IMapper>().Object);
        }

        [Fact]
        public async Task ReturnsAllParticipants()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            var participants = new[]
            {
                new ParticipantDto { Id = "1", Name = "Test1", Status = ParticipantStatusDto.Active },
                new ParticipantDto { Id = "2", Name = "Test2", Status = ParticipantStatusDto.Disabled }
            };
            _ = mockParticipantStore.Setup(m => m.GetAll()).Returns(Task.FromResult(participants.AsEnumerable()));

            var expectedParticipants = new[]
            {
                new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active },
                new ParticipantResponseModel { Id = "2", Name = "Test2", Status = ParticipantStatus.Disabled }
            };

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var values = Assert.IsAssignableFrom<IEnumerable<ParticipantResponseModel>>(okay.Value);
            _ = values.Should().BeEquivalentTo(expectedParticipants);
        }

        [Fact]
        public async Task ReturnsASpecificParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            var participants = new[]
            {
                new ParticipantDto { Id = "1", Name = "Test1", Status = ParticipantStatusDto.Active },
                new ParticipantDto { Id = "2", Name = "Test2", Status = ParticipantStatusDto.Disabled }
            };
            _ = mockParticipantStore.Setup(m => m.GetById(participants[0].Id!)).Returns(Task.FromResult<ParticipantDto?>(participants[0]));

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(participants[0].Id!).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var value = Assert.IsType<ParticipantResponseModel>(okay.Value);
            _ = value.Should().BeEquivalentTo(participants[0]);
        }
    }
}
