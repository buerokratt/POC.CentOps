using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Text;

namespace CentOps.UnitTests
{
    public class PublicParticipantControllerTests
    {
        private readonly MapperConfiguration _mapper;

        private readonly ParticipantDto[] _participantDtos = new[]
            {
                DtoBuilder.GetParticipant(
                    id: "1",
                    name: "Test1",
                    institutionId: "1",
                    type: ParticipantTypeDto.Chatbot,
                    status: ParticipantStatusDto.Active),
                DtoBuilder.GetParticipant(
                    id: "2",
                    name: "Test2",
                    institutionId: "2",
                    type: ParticipantTypeDto.Chatbot,
                    status: ParticipantStatusDto.Disabled),
                DtoBuilder.GetParticipant(
                    id: "3",
                    name: "TestDmr1",
                    institutionId: "1",
                    type: ParticipantTypeDto.Dmr,
                    status: ParticipantStatusDto.Active)
            };

        private readonly ParticipantResponseModel[] _participantResponseModels = new[]
            {
                new ParticipantResponseModel
                {
                    Id = "1",
                    Name = "Test1",
                    InstitutionId = "1",
                    Host = "https://host:8080",
                    Type = ParticipantType.Chatbot,
                    Status = ParticipantStatus.Active
                },
                new ParticipantResponseModel
                {
                    Id = "2",
                    Name = "Test2",
                    InstitutionId = "2",
                    Host = "https://host:8080",
                    Type = ParticipantType.Chatbot,
                    Status = ParticipantStatus.Disabled
                },
                new ParticipantResponseModel
                {
                    Id = "3",
                    Name = "TestDmr1",
                    InstitutionId = "1",
                    Host = "https://host:8080",
                    Type = ParticipantType.Dmr,
                    Status = ParticipantStatus.Active
                },
            };

        public PublicParticipantControllerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
        }

        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new PublicParticipantController(new Mock<IParticipantStore>().Object, new Mock<IMapper>().Object);
        }

        [Fact]
        public async Task GetReturnsAllActiveParticipants()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore
               .Setup(m => m.GetAll(It.Is<IEnumerable<ParticipantTypeDto>>(t => t.Any() == false), false))
               .ReturnsAsync(
                    _participantDtos
                        .Where(p => p.Status == ParticipantStatusDto.Active)
                        .AsEnumerable());

            var sut = CreatePublicParticipantController(
                mockParticipantStore.Object,
                _mapper.CreateMapper(),
                string.Empty,
                _participantDtos[1]);

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var values = Assert.IsAssignableFrom<IEnumerable<ParticipantResponseModel>>(okay.Value);
            _ = values.Should().BeEquivalentTo(_participantResponseModels.Where(p => p.Status == ParticipantStatus.Active));
        }

        [Fact]
        public async Task GetReturnsDmrParticipants()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore
               .Setup(m => m.GetAll(It.Is<IEnumerable<ParticipantTypeDto>>(t => t.Count() == 1 && t.Contains(ParticipantTypeDto.Dmr)), false))
               .ReturnsAsync(
                _participantDtos
                    .Where(p => p.Status == ParticipantStatusDto.Active && p.Type == ParticipantTypeDto.Dmr)
                    .AsEnumerable());

            var sut = CreatePublicParticipantController(
                mockParticipantStore.Object,
                _mapper.CreateMapper(),
                "?type=Dmr",
                _participantDtos[0]);

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var values = Assert.IsAssignableFrom<IEnumerable<ParticipantResponseModel>>(okay.Value);
            _ = values
                .Should()
                .BeEquivalentTo(_participantResponseModels.Where(p => p.Status == ParticipantStatus.Active && p.Type == ParticipantType.Dmr));
        }

        [Fact]
        public async Task GetReturnsASpecificParticipant()
        {
            // Arrange
            var participant = _participantDtos[0];
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.GetById(participant.Id!)).ReturnsAsync(_participantDtos[0]);

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = CreatePublicParticipantController(
                mockParticipantStore.Object,
                _mapper.CreateMapper(),
                participant: participant);

            // Act
            var response = await sut.Get(participant.Id!).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var value = Assert.IsType<ParticipantResponseModel>(okay.Value);
            _ = value.Should().BeEquivalentTo(_participantResponseModels[0]);
        }

        [Fact]
        public async Task GetReturns404ForParticipantNotFound()
        {
            // Arrange
            var participant = _participantDtos[0];

            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.GetById(participant.Id!)).ReturnsAsync((ParticipantDto)null);

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = CreatePublicParticipantController(
                mockParticipantStore.Object,
                _mapper.CreateMapper(),
                participant: participant);

            // Act
            var response = await sut.Get(_participantDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task PutParticipantStatusReturnsParticipantWithUpdatedStatus()
        {
            var id = Guid.NewGuid().ToString();
            var participant = DtoBuilder.GetParticipant(id, status: ParticipantStatusDto.Active);
            var updatedParticipant = DtoBuilder.GetParticipant(id, status: ParticipantStatusDto.Disabled);

            Mock<IParticipantStore> mockParticipantStore = new();
            _ = mockParticipantStore.Setup(x => x.UpdateStatus(participant.Id, ParticipantStatusDto.Disabled)).ReturnsAsync(updatedParticipant);

            var controller = CreatePublicParticipantController(
                mockParticipantStore.Object,
                _mapper.CreateMapper(),
                participant: participant);

            var response = await controller.Put(ParticipantStatus.Disabled).ConfigureAwait(false);

            var okay = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(okay.StatusCode, StatusCodes.Status200OK);
            var value = Assert.IsType<ParticipantStatusReponseModel>(okay.Value);
            Assert.NotNull(value);
            Assert.Equal(value.Id, updatedParticipant.Id);
            Assert.Equal(value.Status.ToString(), updatedParticipant.Status.ToString());
        }

        [Fact]
        public async Task PutParticipantStatusReturns404NotFound()
        {
            var id = Guid.NewGuid().ToString();
            var participant = DtoBuilder.GetParticipant(id, status: ParticipantStatusDto.Active);

            Mock<IParticipantStore> mockParticipantStore = new();
            _ = mockParticipantStore.Setup(x => x.UpdateStatus(participant.Id, ParticipantStatusDto.Disabled)).ThrowsAsync(new ModelNotFoundException<ParticipantDto>());

            var controller = CreatePublicParticipantController(
                mockParticipantStore.Object,
                _mapper.CreateMapper(),
                participant: participant);

            var response = await controller.Put(ParticipantStatus.Disabled).ConfigureAwait(false);

            var notFound = Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.Equal(notFound.StatusCode, StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task PutParticipantStatusReturns400BadRequest()
        {
            var id = Guid.NewGuid().ToString();
            var participant = DtoBuilder.GetParticipant(id: id, status: ParticipantStatusDto.Active);

            Mock<IParticipantStore> mockParticipantStore = new();
            _ = mockParticipantStore.Setup(x => x.UpdateStatus(participant.Id, ParticipantStatusDto.Deleted)).ThrowsAsync(new ArgumentException());

            var controller = CreatePublicParticipantController(
                mockParticipantStore.Object,
                _mapper.CreateMapper(),
                participant: participant);

            var response = await controller.Put(ParticipantStatus.Deleted).ConfigureAwait(false);

            var badRequest = Assert.IsType<BadRequestObjectResult>(response.Result);
            Assert.Equal(badRequest.StatusCode, StatusCodes.Status400BadRequest);
        }

        private static PublicParticipantController CreatePublicParticipantController(
           IParticipantStore store,
           IMapper mapper,
           string queryString = "",
           ParticipantDto participant = null)
        {
            return new PublicParticipantController(store, mapper)
            {
                ControllerContext = new ControllerContext() { HttpContext = GetContext(queryString, participant) }
            };
        }

        private static DefaultHttpContext GetContext(string queryString, ParticipantDto participant)
        {
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.QueryString = new QueryString(queryString);

            var claims = new[]
            {
                new Claim("id", participant.Id!),
                new Claim("pk", participant.PartitionKey!),
                new Claim("name", participant.Name!),
                new Claim("institutionId", participant.InstitutionId!),
                new Claim("status", participant.Status.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            httpContext.User.AddIdentity(identity);

            return httpContext;
        }
    }
}

