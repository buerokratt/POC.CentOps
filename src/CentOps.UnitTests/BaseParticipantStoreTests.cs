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

        private readonly InstitutionDto _defaultCreatedInstitution;
        protected BaseParticipantStoreTests()
        {
            InstitutionFilter = m => true;
            ParticipantFilter = m => true;
            _defaultCreatedInstitution = new InstitutionDto
            {
                Name = "DefaultInstitution",
                Status = InstitutionStatusDto.Active,
                Id = "1234",
                PartitionKey = $"institution::1234"
            };
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

            _ = GetInstitutionStore(_defaultCreatedInstitution);
            var sut = GetParticipantStore();

            var participant = GetParticipant();
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
            _ = GetInstitutionStore(_defaultCreatedInstitution);
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

            var participant = GetParticipant(name: null);

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
            _ = GetInstitutionStore(_defaultCreatedInstitution);
            var sut = GetParticipantStore(seed: GetParticipant());

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ParticipantDto>>(
                    () =>
                    {
                        var participant2 = GetParticipant(); // This creates the same object
                        return sut.Create(participant2);
                    })
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetByIdCanRetrieveStoredParticipant()
        {
            // Arrange
            var createdParticipant = GetParticipant();
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
            var createdParticipant = GetParticipant();
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

            var participant1 = GetParticipant();

            var participant2 = GetParticipant("234", "Test2");

            var sut = GetParticipantStore(participant1, participant2);

            // Act
            var storedItems = await sut.GetAll().ConfigureAwait(false);

            // Assert
            _ = storedItems.Should().BeEquivalentTo(new[] { participant1, participant2 });
        }

        [Fact]
        public async Task DeleteSuccessfullyRemovesParticipants()
        {
            // Arrange
            var participant = GetParticipant();

            var sut = GetParticipantStore(participant);

            // Act
            var deleted = await sut.DeleteById(participant.Id).ConfigureAwait(false);

            var retreivedParticipant = await sut.GetById(participant.Id).ConfigureAwait(false);

            // Assert
            Assert.True(deleted);
            Assert.Null(retreivedParticipant);
        }

        private static InstitutionDto GetInstitution(
            string id = "123",
            string name = "Test",
            InstitutionStatusDto status = InstitutionStatusDto.Active)
        {
            return new InstitutionDto
            {
                Id = id,
                PartitionKey = $"institution::{id}",
                Name = name,
                Status = status,
            };
        }

        private ParticipantDto GetParticipant(
            string id = "123",
            string name = "Test",
            string host = "https://host:8080",
            ParticipantStatusDto status = ParticipantStatusDto.Active,
            ParticipantTypeDto type = ParticipantTypeDto.Chatbot)
        {
            return new ParticipantDto
            {
                Id = id,
                PartitionKey = $"participant::{id}",
                Name = name,
                Host = host,
                Status = status,
                Type = type,
                InstitutionId = _defaultCreatedInstitution.Id
            };
        }
    }
}
