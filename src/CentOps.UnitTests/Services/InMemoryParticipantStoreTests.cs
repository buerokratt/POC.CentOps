using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;

namespace CentOps.UnitTests.Services
{
    public class InMemoryParticipantStoreTests
    {
        private InstitutionDto _defaultCreatedInstitution;

        /// <summary>
        /// Create Participant Store with Default Insitution.
        /// </summary>
        /// <returns>An Async Task wrapping this process.</returns>
        private async Task<IParticipantStore> CreateParticipantStoreAsync()
        {
            var memoryStore = new InMemoryStore() as IInstitutionStore;
            var defaultInstutution = new InstitutionDto
            {
                Name = "DefaultInstitution",
                Status = InstitutionStatusDto.Active
            };

            _defaultCreatedInstitution = await memoryStore.Create(defaultInstutution).ConfigureAwait(false);
            return memoryStore as IParticipantStore;
        }

        [Fact]
        public async Task CreateCanStoreParticipant()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            // Act
            var createdParticipant = await sut.Create(participant).ConfigureAwait(false);

            // Assert
            Assert.NotNull(createdParticipant.Id);
            Assert.Equal(participant.Name, createdParticipant.Name);
            Assert.Equal(participant.Host, createdParticipant.Host);
            Assert.Equal(participant.Status, createdParticipant.Status);
            Assert.Equal(participant.Type, createdParticipant.Type);
            Assert.Equal(participant.InstitutionId, createdParticipant.InstitutionId);
        }

        [Fact]
        public async Task CreateParticipantThrowsIfInsitutionNotFound()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = "DoesntExist"
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelNotFoundException<InstitutionDto>>(
                    async () => await sut.Create(participant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfParticipantIsNull()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentNullException>(
                    async () => await sut.Create(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfParticipantNameIsNull()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant = new ParticipantDto
            {
                Name = null,
                InstitutionId = "1",
                Host = "https://host:8080",
                Status = ParticipantStatusDto.Active,
                Type = ParticipantTypeDto.Chatbot
            };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Create(participant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfParticipantInsitutionIdIsNull()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant = new ParticipantDto
            {
                Name = "Test",
                InstitutionId = null,
                Host = "https://host:8080",
                Status = ParticipantStatusDto.Active,
                Type = ParticipantTypeDto.Chatbot
            };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Create(participant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfParticipantNameAlreadyExists()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant = new ParticipantDto
            {
                Name = "Test",
                InstitutionId = _defaultCreatedInstitution.Id,
                Host = "https://host:8080",
                Status = ParticipantStatusDto.Active,
                Type = ParticipantTypeDto.Chatbot
            };

            // Create the participant twice - the second attempt should fail.
            _ = await sut.Create(participant).ConfigureAwait(false);

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ParticipantDto>>(
                    async () => await sut.Create(participant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetByIdCanRetrieveStoredParticipant()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            var createdParticipant = await sut.Create(participant).ConfigureAwait(false);
            Assert.NotNull(createdParticipant.Id);

            // Act
            var storedItem = await sut.GetById(createdParticipant.Id!).ConfigureAwait(false);

            // Assert
            Assert.NotNull(storedItem);
            Assert.Equal(storedItem?.Id, createdParticipant.Id);
            Assert.Equal(storedItem?.Name, participant.Name);
            Assert.Equal(storedItem?.Host, participant.Host);
            Assert.Equal(storedItem?.Status, participant.Status);
            Assert.Equal(storedItem?.Type, participant.Type);
            Assert.Equal(storedItem?.InstitutionId, participant.InstitutionId);
        }

        [Fact]
        public async Task GetByIdReturnsNullIfParticipantNotFound()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            var createdParticipant = await sut.Create(participant).ConfigureAwait(false);
            Assert.NotNull(createdParticipant.Id);

            // Act
            var storedItem = await sut.GetById("DoesntExist").ConfigureAwait(false);

            // Assert
            Assert.Null(storedItem);
        }

        [Fact]
        public async Task GetAllReturnsAllStoredParticipants()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant1 =
                new ParticipantDto
                {
                    Name = "Test1",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };
            _ = await sut.Create(participant1).ConfigureAwait(false);

            var participant2 =
                new ParticipantDto
                {
                    Name = "Test2",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };
            _ = await sut.Create(participant2).ConfigureAwait(false);

            // Act
            var storedItems = await sut.GetAll().ConfigureAwait(false);

            // Assert
            _ = storedItems.Should().BeEquivalentTo(new[] { participant1, participant2 });
        }

        [Fact]
        public async Task DeleteSuccessfullyRemovesParticipants()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            var createdParticipant = await sut.Create(participant).ConfigureAwait(false);

            // Act
            var deleted = await sut.DeleteById(createdParticipant.Id).ConfigureAwait(false);

            var retreivedParticipant = await sut.GetById(createdParticipant.Id).ConfigureAwait(false);

            // Assert
            Assert.True(deleted);
            Assert.Null(retreivedParticipant);
        }

        [Fact]
        public async Task DeleteIndicatesFailureForNonexistentParticipant()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            // Act
            var deleted = await sut.DeleteById("DoesntExist").ConfigureAwait(false);

            // Assert
            Assert.False(deleted);
        }

        [Fact]
        public async Task DeleteThrowsForNullId()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            // Act & Assert
            _ = await Assert.
                ThrowsAsync<ArgumentException>(
                    async () => await sut.DeleteById(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateCanUpdateAParticipant()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            var createdParticipant = await sut.Create(participant).ConfigureAwait(false);

            var updatedParticipant =
               new ParticipantDto
               {
                   Id = createdParticipant.Id,
                   Name = "UpdatedTest",
                   Host = "https://host:443",
                   Status = ParticipantStatusDto.Disabled,
                   Type = ParticipantTypeDto.Chatbot,
                   InstitutionId = _defaultCreatedInstitution.Id
               };

            // Act
            var storedItem = await sut.Update(updatedParticipant).ConfigureAwait(false);

            // Assert
            Assert.NotNull(storedItem);
            Assert.Equal(storedItem?.Id, updatedParticipant.Id);
            Assert.Equal(storedItem?.Host, updatedParticipant.Host);
            Assert.Equal(storedItem.Status, updatedParticipant.Status);
            Assert.Equal(storedItem?.Name, updatedParticipant.Name);
        }

        [Fact]
        public async Task UpdateThrowsIfInsitutionNotFound()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            var createdParticipant = await sut.Create(participant).ConfigureAwait(false);

            var updatedParticipant =
               new ParticipantDto
               {
                   Id = createdParticipant.Id,
                   Name = "UpdatedTest",
                   Host = "https://host:443",
                   Status = ParticipantStatusDto.Disabled,
                   Type = ParticipantTypeDto.Chatbot,
                   InstitutionId = "DoesntExist"
               };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelNotFoundException<InstitutionDto>>(
                    async () => await sut.Update(updatedParticipant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsIfParticipantNotFound()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            var createdParticipant = await sut.Create(participant).ConfigureAwait(false);

            var updatedParticipant =
               new ParticipantDto
               {
                   Id = "DoesntExist",
                   Name = "UpdatedTest",
                   Host = "https://host:443",
                   Status = ParticipantStatusDto.Disabled,
                   Type = ParticipantTypeDto.Chatbot,
                   InstitutionId = _defaultCreatedInstitution.Id
               };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelNotFoundException<ParticipantDto>>(
                    async () => await sut.Update(updatedParticipant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsIfModelIsNull()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentNullException>(
                    async () => await sut.Update(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsIfModelIdIsNull()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Id = null,
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(participant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsIfNameIsNull()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Id = "1",
                    Name = null,
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(participant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsIfInsitutionIdIsNull()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant =
                new ParticipantDto
                {
                    Id = "1",
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = null
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(participant).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForDuplicateName()
        {
            // Arrange
            var sut = await CreateParticipantStoreAsync().ConfigureAwait(false);

            var participant1 =
                new ParticipantDto
                {
                    Name = "Test1",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };
            var createdParticipant1 = await sut.Create(participant1).ConfigureAwait(false);

            var participant2 =
                new ParticipantDto
                {
                    Name = "Test2",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = _defaultCreatedInstitution.Id
                };

            var createdParticipant2 = await sut.Create(participant2).ConfigureAwait(false);

            var updateWithDuplicate =
                new ParticipantDto
                {
                    Name = createdParticipant1.Name,
                    Id = createdParticipant2.Id,
                    Host = participant2.Host,
                    InstitutionId = participant2.InstitutionId,
                    Type = participant2.Type,
                    Status = participant2.Status
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ParticipantDto>>(
                    async () => await sut.Update(updateWithDuplicate).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
