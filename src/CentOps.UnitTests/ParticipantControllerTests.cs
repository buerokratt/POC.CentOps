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

        private readonly ParticipantDto[] _participantDtos = new[]
            {
                new ParticipantDto { Id = "1", Name = "Test1", InstitutionId = "1", Status = ParticipantStatusDto.Active },
                new ParticipantDto { Id = "2", Name = "Test2", InstitutionId = "2", Status = ParticipantStatusDto.Disabled }
            };

        private readonly ParticipantResponseModel[] _participantResponseModels = new[]
            {
                new ParticipantResponseModel { Id = "1", Name = "Test1", InstitutionId = "1", Status = ParticipantStatus.Active },
                new ParticipantResponseModel { Id = "2", Name = "Test2", InstitutionId = "2", Status = ParticipantStatus.Disabled }
            };

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
            _ = mockParticipantStore.Setup(m => m.GetAll()).Returns(Task.FromResult(_participantDtos.AsEnumerable()));

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var values = Assert.IsAssignableFrom<IEnumerable<ParticipantResponseModel>>(okay.Value);
            _ = values.Should().BeEquivalentTo(_participantResponseModels);
        }

        [Fact]
        public async Task ReturnsASpecificParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.GetById(_participantDtos[0].Id!)).Returns(Task.FromResult<ParticipantDto?>(_participantDtos[0]));

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(_participantDtos[0].Id!).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var value = Assert.IsType<ParticipantResponseModel>(okay.Value);
            _ = value.Should().BeEquivalentTo(_participantResponseModels[0]);
        }

        [Fact]
        public async Task Returns404ForParticipantNotFound()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.GetById(_participantDtos[0].Id!)).Returns(Task.FromResult<ParticipantDto?>(null));

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(_participantDtos[0].Id!).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<NotFoundResult>(response.Result);
        }
    }
}
