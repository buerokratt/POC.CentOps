using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using CentOps.Api.Services.ModelStore.Exceptions;

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
        public async Task GetReturnsAllParticipants()
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
        public async Task GetReturnsASpecificParticipant()
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
        public async Task GetReturns404ForParticipantNotFound()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.GetById(_participantDtos[0].Id!)).Returns(Task.FromResult<ParticipantDto?>(null));

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(_participantDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task PostAddsAParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.Create(It.IsAny<ParticipantDto>())).Returns(Task.FromResult(_participantDtos[0]));

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            var createParticipantModel = new CreateUpdateParticipantModel
            {
                Host = _participantResponseModels[0].Host,
                InstitutionId = _participantResponseModels[0].InstitutionId,
                Name = _participantResponseModels[0].Name,
                Status = _participantResponseModels[0].Status,
                Type = _participantResponseModels[0].Type,
            };

            // Act
            var response = await sut.Post(createParticipantModel).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<CreatedResult>(response.Result);
            _ = _participantResponseModels[0].Should().BeEquivalentTo(okay.Value);
        }

        [Fact]
        public async Task PostReturns409ForExistingParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.Create(It.IsAny<ParticipantDto>())).ThrowsAsync(new ModelExistsException<ParticipantDto>());

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            var createParticipantModel = new CreateUpdateParticipantModel
            {
                Host = _participantResponseModels[0].Host,
                InstitutionId = _participantResponseModels[0].InstitutionId,
                Name = _participantResponseModels[0].Name,
                Status = _participantResponseModels[0].Status,
                Type = _participantResponseModels[0].Type,
            };

            // Act
            var response = await sut.Post(createParticipantModel).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<ConflictResult>(response.Result);
        }

        [Fact]
        public async Task PutUpdatesAParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.Update(It.IsAny<ParticipantDto>())).Returns(Task.FromResult(_participantDtos[0]));

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Put(_participantDtos[0].Id!, new CreateUpdateParticipantModel()).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            _ = _participantResponseModels[0].Should().BeEquivalentTo(okay.Value);
        }

        [Fact]
        public async Task PutReturns404ForUpdatingANonexistentParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.Update(It.IsAny<ParticipantDto>())).ThrowsAsync(new ModelNotFoundException<ParticipantDto>());

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            var updateParticipantModel = new CreateUpdateParticipantModel
            {
                Host = _participantResponseModels[0].Host,
                InstitutionId = _participantResponseModels[0].InstitutionId,
                Name = _participantResponseModels[0].Name,
                Status = _participantResponseModels[0].Status,
                Type = _participantResponseModels[0].Type,
            };

            // Act
            var response = await sut.Put(_participantDtos[0].Id!, updateParticipantModel).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task DeleteRemovesParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.DeleteById(_participantDtos[0].Id!)).Returns(Task.FromResult(true));

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Delete(_participantDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteReturns404ForNonexistentParticipant()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.DeleteById(_participantDtos[0].Id!)).Returns(Task.FromResult(false));

            var sut = new ParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Delete(_participantDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundResult>(response);
        }
    }
}
