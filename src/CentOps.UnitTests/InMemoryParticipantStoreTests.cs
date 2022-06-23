using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Moq;

namespace CentOps.UnitTests
{
    public class InMemoryParticipantStoreTests
    {
        [Fact]
        public async Task CreateCanStoreParticipant()
        {
            // Arrange
            var mock = new Mock<IInstitutionStore>();
            _ = mock.Setup(i => i.GetById("1")).ReturnsAsync(new InstitutionDto { Id = "1" });
            var sut = new InMemoryParticipantStore(mock.Object);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = "1"
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
            var mock = new Mock<IInstitutionStore>();
            var sut = new InMemoryParticipantStore(mock.Object);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = "1"
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
            var mock = new Mock<IInstitutionStore>();
            var sut = new InMemoryParticipantStore(mock.Object);

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
            var mock = new Mock<IInstitutionStore>();
            var sut = new InMemoryParticipantStore(mock.Object);

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
            var mock = new Mock<IInstitutionStore>();
            var sut = new InMemoryParticipantStore(mock.Object);

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
            var mock = new Mock<IInstitutionStore>();
            _ = mock.Setup(i => i.GetById("1")).ReturnsAsync(new InstitutionDto { Id = "1" });

            var sut = new InMemoryParticipantStore(mock.Object);

            var participant = new ParticipantDto
            {
                Name = "Test",
                InstitutionId = "1",
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
            var mock = new Mock<IInstitutionStore>();
            _ = mock.Setup(i => i.GetById("1")).ReturnsAsync(new InstitutionDto { Id = "1" });
            var sut = new InMemoryParticipantStore(mock.Object);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = "1"
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
            var mock = new Mock<IInstitutionStore>();
            _ = mock.Setup(i => i.GetById("1")).ReturnsAsync(new InstitutionDto { Id = "1" });
            var sut = new InMemoryParticipantStore(mock.Object);

            var participant =
                new ParticipantDto
                {
                    Name = "Test",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = "1"
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
            var mock = new Mock<IInstitutionStore>();
            _ = mock.Setup(i => i.GetById("1")).ReturnsAsync(new InstitutionDto { Id = "1" });
            var sut = new InMemoryParticipantStore(mock.Object);

            var participant1 =
                new ParticipantDto
                {
                    Name = "Test1",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = "1"
                };
            var createdParticipant1 = await sut.Create(participant1).ConfigureAwait(false);

            var participant2 =
                new ParticipantDto
                {
                    Name = "Test2",
                    Host = "https://host:8080",
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot,
                    InstitutionId = "1"
                };
            var createdParticipant2 = await sut.Create(participant2).ConfigureAwait(false);

            // Act
            var storedItems = await sut.GetAll().ConfigureAwait(false);

            // Assert
            Assert.NotNull(storedItems);
            Assert.Equal(2, storedItems.Count());
        }
    }
}
