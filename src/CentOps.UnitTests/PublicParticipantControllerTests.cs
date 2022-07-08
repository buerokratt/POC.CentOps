using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Models;
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
                new ParticipantDto { Id = "1", Name = "Test1", InstitutionId = "1", Status = ParticipantStatusDto.Active },
                new ParticipantDto { Id = "2", Name = "Test2", InstitutionId = "2", Status = ParticipantStatusDto.Disabled }
            };

        private readonly ParticipantResponseModel[] _participantResponseModels = new[]
            {
                new ParticipantResponseModel { Id = "1", Name = "Test1", InstitutionId = "1", Status = ParticipantStatus.Active },
                new ParticipantResponseModel { Id = "2", Name = "Test2", InstitutionId = "2", Status = ParticipantStatus.Disabled }
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
        public async Task GetReturnsAllParticipants()
        {
            // Arrange
            var mockParticipantStore = new Mock<IParticipantStore>();
            _ = mockParticipantStore
               .Setup(m => m.GetAll(It.IsAny<IEnumerable<ParticipantTypeDto>>(), false))
               .ReturnsAsync(_participantDtos.Where(p => p.Status == ParticipantStatusDto.Active).AsEnumerable());

            var sut = CreatePublicParticipantController(mockParticipantStore.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var values = Assert.IsAssignableFrom<IEnumerable<ParticipantResponseModel>>(okay.Value);
            _ = values.Should().BeEquivalentTo(_participantResponseModels.Where(p => p.Status == ParticipantStatus.Active));
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

