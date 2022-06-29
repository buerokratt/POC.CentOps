using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CentOps.UnitTests
{
    public class PublicInstitutionControllerTests
    {
        private readonly MapperConfiguration _mapper;
        private readonly InstitutionDto[] _institutionsDtos = new[]
            {
                new InstitutionDto { Id = "1", Name = "Test1", Status = InstitutionStatusDto.Active },
                new InstitutionDto { Id = "2", Name = "Test2", Status = InstitutionStatusDto.Disabled }
            };
        private readonly InstitutionResponseModel[] _institutionsResponseModels = new[]
            {
                new InstitutionResponseModel { Id = "1", Name = "Test1", Status = InstitutionStatus.Active },
                new InstitutionResponseModel { Id = "2", Name = "Test2", Status = InstitutionStatus.Disabled }
            };

        public PublicInstitutionControllerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
        }

        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new PublicInstitutionController(new Mock<IInstitutionStore>().Object, new Mock<IMapper>().Object);
        }

        [Fact]
        public async Task ReturnsAllInstitutions()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.GetAll()).Returns(Task.FromResult(_institutionsDtos.AsEnumerable()));

            var sut = new PublicInstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var values = Assert.IsAssignableFrom<IEnumerable<InstitutionResponseModel>>(okay.Value);
            _ = values.Should().BeEquivalentTo(_institutionsResponseModels);
        }

        [Fact]
        public async Task ReturnsInstitutionById()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            var institutionId = "1";
            _ = mockInstitutions.Setup(m => m.GetById(institutionId)).Returns(Task.FromResult(_institutionsDtos.FirstOrDefault(x => x.Id == institutionId)));
            var sut = new PublicInstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(institutionId).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var value = Assert.IsType<InstitutionResponseModel>(okay.Value);
            _ = value.Should().BeEquivalentTo(_institutionsResponseModels.Single(i => i.Id == institutionId));
        }

        [Fact]
        public async Task GetReturns404ForInstitutionNotFound()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            var randomInstitutionId = "999";
            _ = mockInstitutions.Setup(m => m.GetById(randomInstitutionId)).Returns(Task.FromResult<InstitutionDto>(null));

            var sut = new PublicInstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(randomInstitutionId).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetParticipantsByInstitutionIdReturnsParticipants()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            var mockParticipatns = new Mock<IParticipantStore>();
            var institutionId = "1";
            _ = mockInstitutions.Setup(m => m.GetById(institutionId)).Returns(Task.FromResult(_institutionsDtos.FirstOrDefault(x => x.Id == institutionId)));
            var sut = new PublicInstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.GetParticipantsByInstitutionId(institutionId).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            var values = Assert.IsAssignableFrom<IEnumerable<ParticipantResponseModel>>(okay.Value);
            _ = values.Should().BeEquivalentTo(Array.Empty<object>());
        }
    }
}

