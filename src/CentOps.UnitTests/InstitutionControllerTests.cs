using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CentOps.UnitTests
{
    public class InstitutionControllerTests
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

        public InstitutionControllerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
        }

        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new InstitutionController(new Mock<IInstitutionStore>().Object, new Mock<IMapper>().Object);
        }

        [Fact]
        public async Task ReturnsAllInstitutions()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.GetAll()).Returns(Task.FromResult(_institutionsDtos.AsEnumerable()));

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

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
            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

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

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Get(randomInstitutionId).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task PostAddsAInstitution()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.Create(It.IsAny<InstitutionDto>())).ReturnsAsync(_institutionsDtos[0]);

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            var createInstitutionModel = new CreateUpdateInsitutionModel
            {
                Name = _institutionsResponseModels[0].Name,
                Status = _institutionsResponseModels[0].Status
            };

            // Act
            var response = await sut.Post(createInstitutionModel).ConfigureAwait(false);

            // Assert
            var created = Assert.IsType<CreatedResult>(response.Result);
            _ = _institutionsResponseModels[0].Should().BeEquivalentTo(created.Value);
        }

        [Fact]
        public async Task PostReturns409ForExistingInstitution()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.Create(It.IsAny<InstitutionDto>())).ThrowsAsync(new ModelExistsException<InstitutionDto>());

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            var createInstitutionModel = new CreateUpdateInsitutionModel
            {
                Name = _institutionsResponseModels[0].Name,
                Status = _institutionsResponseModels[0].Status
            };

            // Act
            var response = await sut.Post(createInstitutionModel).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<ConflictResult>(response.Result);
        }
        [Fact]
        public async Task PutUpdatesAInstitution()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.Update(It.IsAny<InstitutionDto>())).ReturnsAsync(_institutionsDtos[0]);

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Put(_institutionsDtos[0].Id!, new CreateUpdateInsitutionModel()).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            _ = _institutionsResponseModels[0].Should().BeEquivalentTo(okay.Value);
        }

        [Fact]
        public async Task PutReturns404ForUpdatingANonexistentInstitution()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.Update(It.IsAny<InstitutionDto>())).ThrowsAsync(new ModelNotFoundException<InstitutionDto>());

            var expectedParticipant = new InstitutionResponseModel { Id = "1", Name = "Test1", Status = InstitutionStatus.Active };

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            var updateInstitutionModel = new CreateUpdateInsitutionModel
            {
                Name = _institutionsResponseModels[0].Name,
                Status = _institutionsResponseModels[0].Status,
            };

            // Act
            var response = await sut.Put(_institutionsDtos[0].Id!, updateInstitutionModel).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response.Result);
        }


        [Fact]
        public async Task PutReturns400ForAnImproperInstitution()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.Update(It.IsAny<InstitutionDto>())).ThrowsAsync(new ArgumentException());

            var expectedParticipant = new ParticipantResponseModel { Id = "1", Name = "Test1", Status = ParticipantStatus.Active };

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            var updateInstitutionModel = new CreateUpdateInsitutionModel
            {
                Name = null,
                Status = _institutionsResponseModels[0].Status,
            };

            // Act
            var response = await sut.Put(_institutionsDtos[0].Id!, updateInstitutionModel).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task DeleteRemovesInstitution()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.DeleteById(_institutionsDtos[0].Id!)).ReturnsAsync(true);

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Delete(_institutionsDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteReturns404ForNonexistentInstitution()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.DeleteById(_institutionsDtos[0].Id!)).ReturnsAsync(false);

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Delete(_institutionsDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async Task DeleteReturns400ForNullId()
        {
            // Arrange
            var mockInstitutions = new Mock<IInstitutionStore>();
            _ = mockInstitutions.Setup(m => m.DeleteById(_institutionsDtos[0].Id!)).ThrowsAsync(new ArgumentException());

            var sut = new InstitutionController(mockInstitutions.Object, _mapper.CreateMapper());

            // Act
            var response = await sut.Delete(_institutionsDtos[0].Id!).ConfigureAwait(false);

            // Assert
            _ = Assert.IsType<BadRequestObjectResult>(response);
        }
    }
}
