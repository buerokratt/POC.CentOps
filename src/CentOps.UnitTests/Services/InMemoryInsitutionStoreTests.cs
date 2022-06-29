using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;

namespace CentOps.UnitTests.Services
{
    public class InMemoryInsitutionStoreTests
    {
        [Fact]
        public async Task CreateCanStoreInstitution()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution =
                new InstitutionDto
                {
                    Name = "Test",
                    Status = InstitutionStatusDto.Active
                };

            // Act
            var createdInstitution = await sut.Create(institution).ConfigureAwait(false);

            // Assert
            Assert.NotNull(createdInstitution.Id);
            Assert.Equal(institution.Name, createdInstitution.Name);
            Assert.Equal(createdInstitution.Status, createdInstitution.Status);
        }

        [Fact]
        public async Task CreateThrowsIfInstitutiontIsNull()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentNullException>(
                    async () => await sut.Create(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfInstitutionNameIsNull()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution = new InstitutionDto { Name = null, Status = InstitutionStatusDto.Active };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Create(institution).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfDuplicateNameSpecified()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution1 = new InstitutionDto { Name = "Test", Status = InstitutionStatusDto.Active };
            _ = await sut.Create(institution1).ConfigureAwait(false);

            var institution2 = new InstitutionDto { Name = institution1.Name, Status = InstitutionStatusDto.Active };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<InstitutionDto>>(
                    async () => await sut.Create(institution2).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteThrowsIfIdIsNull()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.DeleteById(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteReturnsFalseIfInstitutionDoesntExist()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            // Act & Assert
            var response = await sut.DeleteById("DoesntExist").ConfigureAwait(false);

            Assert.False(response);
        }

        [Fact]
        public async Task DeleteReturnsTrueIfSuccessful()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution = new InstitutionDto { Name = "Test", Status = InstitutionStatusDto.Active };

            var created = await sut.Create(institution).ConfigureAwait(false);

            // Act
            var response = await sut.DeleteById(created.Id).ConfigureAwait(false);

            // Assert
            Assert.True(response);

            var found = await sut.GetById(created.Id).ConfigureAwait(false);
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteFailsIfParticipantsExistForInstitution()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution = new InstitutionDto { Name = "Test", Status = InstitutionStatusDto.Active };
            var createdInstitution = await sut.Create(institution).ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://participant:8080",
                    InstitutionId = createdInstitution.Id,
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot
                };

            var createdParticipant = await ((IParticipantStore)sut).Create(participant).ConfigureAwait(false);

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ParticipantDto>>(
                    async () => await sut.DeleteById(createdInstitution.Id).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetAllReturnsAllStoredInsitutions()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution1 = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active };
            _ = await sut.Create(institution1).ConfigureAwait(false);

            var institution2 = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Active };
            _ = await sut.Create(institution2).ConfigureAwait(false);

            // Act
            var response = await sut.GetAll().ConfigureAwait(false);

            // Assert
            _ = response.Should().BeEquivalentTo(new[] { institution1, institution2 });
        }

        [Fact]
        public async Task GetParticipantsByInstitutionIdReturnsAllParticipantWithInstitutionId()
        {
            // Arrange
            var memoryStore = new InMemoryStore() as IInstitutionStore;

            var institutionDto1 = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active };
            var institution1 = await memoryStore.Create(institutionDto1).ConfigureAwait(false);

            var institutionDto2 = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Active };
            var institution2 = await memoryStore.Create(institutionDto2).ConfigureAwait(false);

            var participantStore = memoryStore as IParticipantStore;

            var participantDto1 = new ParticipantDto
            {
                Name = "Test1",
                Host = "https://host:8080",
                Status = ParticipantStatusDto.Active,
                Type = ParticipantTypeDto.Chatbot,
                InstitutionId = institution1.Id
            };
            var participant1 = await participantStore.Create(participantDto1).ConfigureAwait(false);

            var participantDto2 = new ParticipantDto
            {
                Name = "Test2",
                Host = "https://host:8080",
                Status = ParticipantStatusDto.Active,
                Type = ParticipantTypeDto.Dmr,
                InstitutionId = institution1.Id
            };
            var participant2 = await participantStore.Create(participantDto2).ConfigureAwait(false);

            var participantDto3 = new ParticipantDto
            {
                Name = "Test3",
                Host = "https://different-host:8080",
                Status = ParticipantStatusDto.Active,
                Type = ParticipantTypeDto.Classifier,
                InstitutionId = institution2.Id
            };
            _ = await participantStore.Create(participantDto3).ConfigureAwait(false);

            // Act
            var response = await memoryStore.GetParticipantsByInstitutionId(institution1.Id).ConfigureAwait(false);
            //var response = await institutionStore.GetById(institution1.Id).ConfigureAwait(false);

            // Assert
            _ = response.Should().BeEquivalentTo(new[] { participant1, participant2 });
            //_ = response.Should().BeEquivalentTo(institution1);

        }

        [Fact]
        public async Task GetParticipantsByInstitutionIdThrowsIfIfIdIsNull()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.GetParticipantsByInstitutionId("").ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateCanUpdateAnInstitution()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active };
            var createdInstitution = await sut.Create(institution).ConfigureAwait(false);

            // Act
            var institutionWithUpdates = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Disabled, Id = createdInstitution.Id };
            var updatedInstitution = await sut.Update(institutionWithUpdates).ConfigureAwait(false);

            Assert.Equal(institutionWithUpdates.Id, updatedInstitution.Id);
            Assert.Equal(institutionWithUpdates.Name, updatedInstitution.Name);
            Assert.Equal(institutionWithUpdates.Status, updatedInstitution.Status);
        }

        [Fact]
        public async Task UpdateThrowsForNullModel()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentNullException>(
                    async () => await sut.Update(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForNullInstitutionName()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution = new InstitutionDto { Name = null, Status = InstitutionStatusDto.Disabled, Id = "1" };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(institution).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForNullInsitutionId()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution = new InstitutionDto { Name = "Name", Status = InstitutionStatusDto.Disabled, Id = null };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(institution).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForNonexistentInstitution()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active, Id = "DoesntExist" };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelNotFoundException<InstitutionDto>>(
                    async () => await sut.Update(institution).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForDuplicateName()
        {
            // Arrange
            var sut = new InMemoryStore() as IInstitutionStore;

            var institution1 = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active };
            var createdInstitution1 = await sut.Create(institution1).ConfigureAwait(false);

            var institution2 = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Active };
            var createdInstitution2 = await sut.Create(institution2).ConfigureAwait(false);

            var institutionWithUpdates =
                new InstitutionDto
                {
                    Name = institution1.Name,
                    Status = institution2.Status,
                    Id = createdInstitution2.Id
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<InstitutionDto>>(
                    async () => await sut.Update(institutionWithUpdates).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
