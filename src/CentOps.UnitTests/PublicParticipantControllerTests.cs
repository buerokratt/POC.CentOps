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
using System.Text;

namespace CentOps.UnitTests
{
    public class PublicParticipantControllerTests
    {
        private readonly MapperConfiguration _mapper;

        private readonly ParticipantDto[] _participantDtos = new[]
            {
                new ParticipantDto
                {
                    Id = "1",
                    Name = "Test1",
                    InstitutionId = "1",
                    Type = ParticipantTypeDto.Chatbot,
                    Status = ParticipantStatusDto.Active
                },
                new ParticipantDto
                {
                    Id = "2",
                    Name = "Test2",
                    InstitutionId = "2",
                    Type = ParticipantTypeDto.Chatbot,
                    Status = ParticipantStatusDto.Disabled
                },
                new ParticipantDto
                {
                    Id = "3",
                    Name = "TestDmr1",
                    InstitutionId = "1",
                    Type = ParticipantTypeDto.Dmr,
                    Status = ParticipantStatusDto.Active,
                }
            };

        private readonly ParticipantResponseModel[] _participantResponseModels = new[]
            {
                new ParticipantResponseModel
                {
                    Id = "1",
                    Name = "Test1",
                    InstitutionId = "1",
                    Type = ParticipantType.Chatbot,
                    Status = ParticipantStatus.Active
                },
                new ParticipantResponseModel
                {
                    Id = "2",
                    Name = "Test2",
                    InstitutionId = "2",
                    Type = ParticipantType.Chatbot,
                    Status = ParticipantStatus.Disabled
                },
                new ParticipantResponseModel
                {
                    Id = "3",
                    Name = "TestDmr1",
                    InstitutionId = "1",
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
                string.Empty);

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
                "?type=Dmr");

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
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore.Setup(m => m.GetById(_participantDtos[0].Id!)).ReturnsAsync(_participantDtos[0]);

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = CreatePublicParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

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
            _ = mockParticipantStore.Setup(m => m.GetById(_participantDtos[0].Id!)).ReturnsAsync((ParticipantDto)null);

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = CreatePublicParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(_participantDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task PutParticipantStateReturnsParticipantWithUpdatedState()
        {
            var id = Guid.NewGuid().ToString();
            var participant = CreateParticipant(id, ParticipantState.Online);
            var updatedParticipant = CreateParticipant(id, ParticipantState.Offline);

            Mock<IParticipantStore> mockParticipantStore = new();
            _ = mockParticipantStore.Setup(x => x.UpdateState(participant.Id, ParticipantState.Offline)).ReturnsAsync(updatedParticipant);


            var controller = CreatePublicParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());
            var response = await controller.Put(participant.Id, ParticipantState.Offline).ConfigureAwait(false);

            var okay = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(okay.StatusCode, StatusCodes.Status200OK);
            var value = Assert.IsType<ParticipantStateReponseModel>(okay.Value);
            Assert.NotNull(value);
            Assert.Equal(value.Id, updatedParticipant.Id);
            Assert.Equal(value.State, updatedParticipant.State);
        }

        [Fact]
        public async Task PutParticipantStateReturns404NotFound()
        {
            var id = Guid.NewGuid().ToString();
            var participant = CreateParticipant(id, ParticipantState.Online);

            Mock<IParticipantStore> mockParticipantStore = new();
            _ = mockParticipantStore.Setup(x => x.UpdateState(participant.Id, ParticipantState.Offline)).ThrowsAsync(new ModelNotFoundException<ParticipantDto>());

            var controller = CreatePublicParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());
            var response = await controller.Put(participant.Id, ParticipantState.Offline).ConfigureAwait(false);

            var notFound = Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.Equal(notFound.StatusCode, StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task PutParticipantStateReturns400BadRequest()
        {
            var id = Guid.NewGuid().ToString();
            var participant = CreateParticipant(id, ParticipantState.Online);

            Mock<IParticipantStore> mockParticipantStore = new();
            _ = mockParticipantStore.Setup(x => x.UpdateState(participant.Id, ParticipantState.Deleted)).ThrowsAsync(new ArgumentException());

            var controller = CreatePublicParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());
            var response = await controller.Put(participant.Id, ParticipantState.Deleted).ConfigureAwait(false);

            var badRequest = Assert.IsType<BadRequestObjectResult>(response.Result);
            Assert.Equal(badRequest.StatusCode, StatusCodes.Status400BadRequest);
        }

        private static ParticipantDto CreateParticipant(string id, ParticipantState state)
        {
            return new ParticipantDto { Id = id, State = state };
        }

        private static PublicParticipantController CreatePublicParticipantController(
           IParticipantStore store,
           IMapper mapper,
           string queryString = "")
        {
            return new PublicParticipantController(store, mapper)
            {
                ControllerContext = new ControllerContext() { HttpContext = GetContext(queryString) }
            };
        }

        private static DefaultHttpContext GetContext(string queryString)
        {
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;
            httpContext.Request.QueryString = new QueryString(queryString);
            return httpContext;
        }
    }
}

