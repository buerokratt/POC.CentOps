using AutoMapper;
using CentOps.Api;
using CentOps.Api.Controllers;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CentOps.UnitTests
{
    public class InstitutionControllerTests
    {
        [Fact]
        public void CreatesParticipantControllerWithoutThrowing()
        {
            _ = new InstitutionController(new Mock<IInstitutionStore>().Object, new Mock<IMapper>().Object);
        }

        [Fact]
        public async Task ReturnsAllInstitutions()
        {
            // Arrange
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            var mockInstitutions = new Mock<IInstitutionStore>();
            var institutions = new[]
            {
                new InstitutionDto { Id = "1", Name = "Test1", Status = InstitutionStatusDto.Active },
                new InstitutionDto { Id = "2", Name = "Test2", Status = InstitutionStatusDto.Disabled }
            };
            _ = mockInstitutions.Setup(m => m.GetAll()).Returns(Task.FromResult(institutions.AsEnumerable()));

            var sut = new InstitutionController(mockInstitutions.Object, mapper.CreateMapper());

            // Act
            var response = await sut.Get().ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(institutions, okay.Value);
        }

        [Fact]
        public async Task ReturnsInstitutionById()
        {
            // Arrange
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            var mockInstitutions = new Mock<IInstitutionStore>();
            var institutions = new[]
            {
                new InstitutionDto { Id = "1", Name = "Test1", Status = InstitutionStatusDto.Active },
                new InstitutionDto { Id = "2", Name = "Test2", Status = InstitutionStatusDto.Disabled }
            };
            var institutionId = "1";
            _ = mockInstitutions.Setup(m => m.GetById(institutionId)).Returns(Task.FromResult(institutions.FirstOrDefault(x => x.Id == institutionId)));
            var sut = new InstitutionController(mockInstitutions.Object, mapper.CreateMapper());

            // Act
            var response = await sut.Get(institutionId).ConfigureAwait(false);

            // Assert
            var okay = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(institutions.Single(x => x.Id == institutionId), okay.Value);
        }
    }
}
