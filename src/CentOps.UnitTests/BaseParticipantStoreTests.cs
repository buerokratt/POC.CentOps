using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using System.Linq.Expressions;

namespace CentOps.UnitTests
{
    public abstract class BaseParticipantStoreTests
    {
        protected Expression<Func<InstitutionDto, bool>> InstitutionFilter { get; private set; }

        protected Expression<Func<ParticipantDto, bool>> ParticipantFilter { get; private set; }

        protected BaseParticipantStoreTests()
        {
            InstitutionFilter = m => true;
            ParticipantFilter = m => true;
        }

        protected abstract IInstitutionStore GetInstitutionStore(params InstitutionDto[] seed);

        protected abstract IParticipantStore GetParticipantStore(params ParticipantDto[] seed);

        protected void SetupInstitutionQuery(Expression<Func<InstitutionDto, bool>> filter)
        {
            InstitutionFilter = filter;
        }

        protected void SetupParticipantQuery(Expression<Func<ParticipantDto, bool>> filter)
        {
            ParticipantFilter = filter;
        }

        [Fact]
        public virtual async Task CreateCanStoreParticipant()
        {
            // Arrange
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);
            var sut = GetParticipantStore();

            var participant = DtoBuilder.GetParticipant();
            participant.Id = null;
            participant.PartitionKey = null;

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
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);
            var sut = GetParticipantStore();

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
            var sut = GetParticipantStore();

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
            var sut = GetParticipantStore();

            var participant = DtoBuilder.GetParticipant(name: null);

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
            var sut = GetParticipantStore();

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
        public async Task CreateThrowsIfDuplicateNameSpecified()
        {
            // Arrange
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);
            var sut = GetParticipantStore(seed: DtoBuilder.GetParticipant());

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ParticipantDto>>(
                    () =>
                    {
                        var participant2 = DtoBuilder.GetParticipant(); // This creates the same object
                        return sut.Create(participant2);
                    })
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetByIdCanRetrieveStoredParticipant()
        {
            // Arrange
            var createdParticipant = DtoBuilder.GetParticipant();
            var sut = GetParticipantStore(createdParticipant);

            // Act
            var storedItem = await sut.GetById(createdParticipant.Id!).ConfigureAwait(false);

            // Assert
            Assert.NotNull(storedItem);
            Assert.Equal(storedItem?.Id, createdParticipant.Id);
            Assert.Equal(storedItem?.Name, createdParticipant.Name);
            Assert.Equal(storedItem?.Host, createdParticipant.Host);
            Assert.Equal(storedItem?.Status, createdParticipant.Status);
            Assert.Equal(storedItem?.Type, createdParticipant.Type);
            Assert.Equal(storedItem?.InstitutionId, createdParticipant.InstitutionId);
        }

        [Fact]
        public async Task GetByIdReturnsNullIfParticipantNotFound()
        {
            // Arrange
            var createdParticipant = DtoBuilder.GetParticipant();
            var sut = GetParticipantStore(createdParticipant);

            // Act
            var storedItem = await sut.GetById("DoesntExist").ConfigureAwait(false);

            // Assert
            Assert.Null(storedItem);
        }

        [Fact]
        public async Task GetAllReturnsAllStoredParticipants()
        {
            // Arrange

            var participant1 = DtoBuilder.GetParticipant();

            var participant2 = DtoBuilder.GetParticipant("234", "Test2");

            var sut = GetParticipantStore(participant1, participant2);

            // Act
            var storedItems = await sut.GetAll().ConfigureAwait(false);

            // Assert
            _ = storedItems.Should().BeEquivalentTo(new[] { participant1, participant2 });
        }

        //[Fact]
        //public async Task GetAllWithFiltersReturnsParticipants()
        //{
        //    // Arrange

        //    var participant1 = DtoHelpers.GetParticipant("1", "bot1");
        //    var participant2 = DtoHelpers.GetParticipant("2", "Dmr", type: ParticipantTypeDto.Dmr);

        //    var sut = GetParticipantStore(participant1, participant2);

        //    // Act
        //    var storedItems = await sut.GetAll(new[] { ParticipantTypeDto.Chatbot }, true).ConfigureAwait(false);

        //    // Assert
        //    _ = storedItems.Should().BeEquivalentTo(new[] { participant1 });
        //}

        [Fact]
        public async Task DeleteSuccessfullyRemovesParticipants()
        {
            // Arrange
            var participant = DtoBuilder.GetParticipant();

            var sut = GetParticipantStore(participant);

            // Act
            var deleted = await sut.DeleteById(participant.Id).ConfigureAwait(false);

            var retreivedParticipant = await sut.GetById(participant.Id).ConfigureAwait(false);

            // Assert
            Assert.True(deleted);
            Assert.Null(retreivedParticipant);
        }

        [Fact]
        public async Task DeleteIndicatesFailureForNonexistentParticipant()
        {
            // Arrange
            var sut = GetParticipantStore();

            // Act
            var deleted = await sut.DeleteById("DoesntExist").ConfigureAwait(false);

            // Assert
            Assert.False(deleted);
        }

        [Fact]
        public async Task DeleteThrowsForNullId()
        {
            // Arrange
            var sut = GetParticipantStore();

            // Act & Assert
            _ = await Assert.
                ThrowsAsync<ArgumentNullException>(
                    async () => await sut.DeleteById(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public virtual async Task UpdateCanUpdateAParticipant()
        {
            // Arrange
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);
            var participant = DtoBuilder.GetParticipant(name: "Test1");
            var sut = GetParticipantStore(seed: participant);

            SetupParticipantQuery(m => m.Id == participant.Id && m.Name == "Test2");

            // Act
            var participantWithUpdates = DtoBuilder.GetParticipant(id: participant.Id, name: "Test2", status: ParticipantStatusDto.Disabled);
            var updatedParticipant = await sut.Update(participantWithUpdates).ConfigureAwait(false);

            Assert.Equal(participantWithUpdates.Id, updatedParticipant.Id);
            Assert.Equal(participantWithUpdates.Name, updatedParticipant.Name);
            Assert.Equal(participantWithUpdates?.Host, updatedParticipant.Host);
            Assert.Equal(participantWithUpdates.Status, updatedParticipant.Status);
        }

        [Fact]
        public async Task UpdateThrowsIfInsitutionNotFound()
        {
            // Arrange
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);
            var createdParticipant = DtoBuilder.GetParticipant();
            var sut = GetParticipantStore();
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
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);
            var sut = GetParticipantStore();

            var updatedParticipant =
               new ParticipantDto
               {
                   Id = "DoesntExist",
                   Name = "UpdatedTest",
                   Host = "https://host:443",
                   Status = ParticipantStatusDto.Disabled,
                   Type = ParticipantTypeDto.Chatbot,
                   InstitutionId = institution.Id
               };

            SetupParticipantQuery(m => m.Id == updatedParticipant.Id);

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
            var sut = GetParticipantStore();

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
            var institution = DtoBuilder.GetInstitution();
            var sut = GetParticipantStore();

            var participant =
                new ParticipantDto
                {
                    Id = null,
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = institution.Id
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
            var institution = DtoBuilder.GetInstitution();
            var sut = GetParticipantStore();

            var participant =
                new ParticipantDto
                {
                    Id = "1",
                    Name = null,
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = institution.Id
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
            var sut = GetParticipantStore();

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
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);

            // Arrange
            var participant1 = DtoBuilder.GetParticipant(id: "123", name: "Test1");
            var participant2 = DtoBuilder.GetParticipant(id: "234", name: "Test2");
            var sut = GetParticipantStore(participant1, participant2);

            var participantWithUpdates =
                new ParticipantDto
                {
                    Id = participant2.Id,
                    Name = participant1.Name,
                    InstitutionId = participant1.InstitutionId,
                    Status = participant2.Status
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ParticipantDto>>(
                    async () => await sut.Update(participantWithUpdates).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetByApiKeyShouldReturnParticipantnWhenParticipantWithApiKeyExists()
        {
            var participant = DtoBuilder.GetParticipant();
            var sut = GetParticipantStore(participant);
            SetupParticipantQuery(x => x.ApiKey == "supersecret_123");

            var fetchedParticipant = await sut.GetByApiKeyAsync("supersecret_123").ConfigureAwait(false);

            Assert.Equal(participant.Id, fetchedParticipant.Id);
            Assert.Equal("supersecret_123", fetchedParticipant.ApiKey);
        }

        [Fact]
        public async Task GetByApiKeyShouldReturnNullWhenApiKeyDoesNotExist()
        {
            var participant = DtoBuilder.GetParticipant();
            var sut = GetParticipantStore(participant);
            SetupParticipantQuery(x => x.ApiKey == "doesnt_query");

            var fetchedParticipant = await sut.GetByApiKeyAsync("doesnt_exist").ConfigureAwait(false);

            Assert.Null(fetchedParticipant);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetByApiKeyShouldThrowWhenApiKeyIsNullOrEmpty(string apiKey)
        {
            var institution = DtoBuilder.GetParticipant();
            var sut = GetParticipantStore(institution);

            _ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetByApiKeyAsync(apiKey))
                .ConfigureAwait(false);
        }

        [Fact(Skip = "The PatchOperation list in PatchItemAsync is currently not mockable. Skipping this test until we can mock it.")]
        public async Task UpdateParticipantStatusFromOfflineToOnlineShouldReturnParticipant()
        {
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);

            var participant = DtoBuilder.GetParticipant(status: ParticipantStatusDto.Disabled);
            var sut = GetParticipantStore(participant);

            SetupParticipantQuery(m => m.Id == participant.Id && m.Name == "Test2");

            var updatedParticipant = await sut.UpdateStatus(participant.Id, ParticipantStatusDto.Active).ConfigureAwait(false);

            Assert.NotNull(updatedParticipant);
            Assert.Equal(participant.Id, updatedParticipant.Id);
            Assert.Equal(ParticipantStatusDto.Active, updatedParticipant.Status);
        }

        [Fact(Skip = "The PatchOperation list in PatchItemAsync is currently not mockable. Skipping this test until we can mock it.")]
        public async Task UpdateParticipantStatusFromOnlineToOfflineShouldReturnParticipant()
        {
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);

            var participant = DtoBuilder.GetParticipant(name: "test12345", status: ParticipantStatusDto.Active);
            var sut = GetParticipantStore(participant);

            SetupParticipantQuery(m => m.Id == participant.Id && m.Name == "Test2");

            var updatedParticipant = await sut.UpdateStatus(participant.Id, ParticipantStatusDto.Disabled).ConfigureAwait(false);

            Assert.NotNull(updatedParticipant);
            Assert.Equal(participant.Id, updatedParticipant.Id);
            Assert.Equal(ParticipantStatusDto.Disabled, updatedParticipant.Status);
        }

        [Fact]
        public async Task UpdateParticipantStatusShouldFailWithInvalidNewStatus()
        {
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);

            var participant = DtoBuilder.GetParticipant(status: ParticipantStatusDto.Active);
            var sut = GetParticipantStore(participant);

            _ = await Assert.ThrowsAnyAsync<ArgumentException>(() => sut.UpdateStatus(participant.Id, ParticipantStatusDto.Deleted)).ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateParticipantStatusShouldFailWithNonExistingId()
        {
            var institution = DtoBuilder.GetInstitution();
            _ = GetInstitutionStore(institution);

            var participant = DtoBuilder.GetParticipant();
            var sut = GetParticipantStore(participant);
            var nonExistingId = "543";

            _ = await Assert.ThrowsAnyAsync<ModelNotFoundException<ParticipantDto>>(() => sut.UpdateStatus(nonExistingId, ParticipantStatusDto.Disabled)).ConfigureAwait(false);
        }
    }
}
