using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using System.Linq.Expressions;

namespace CentOps.UnitTests
{
    public abstract class BaseInstitutionStoreTests
    {
        protected Expression<Func<InstitutionDto, bool>> InstitutionFilter { get; private set; }

        protected Expression<Func<ParticipantDto, bool>> ParticipantFilter { get; private set; }

        protected BaseInstitutionStoreTests()
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
        public virtual async Task CreateCanStoreInstitution()
        {
            // Arrange
            var sut = GetInstitutionStore();

            var institution = GetInstitution();
            institution.Id = null;
            institution.PartitionKey = null;

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
            var sut = GetInstitutionStore();

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
            var sut = GetInstitutionStore();

            var institution = GetInstitution(name: null);

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
            var sut = GetInstitutionStore(seed: GetInstitution());

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<InstitutionDto>>(
                    () =>
                    {
                        var institution2 = GetInstitution(); // This creates the same object
                        return sut.Create(institution2);
                    })
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteThrowsIfIdIsNull()
        {
            // Arrange
            var sut = GetInstitutionStore();

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentNullException>(
                    async () => await sut.DeleteById(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteReturnsFalseIfInstitutionDoesntExist()
        {
            // Arrange
            var sut = GetInstitutionStore();

            // Act & Assert
            var response = await sut.DeleteById("DoesntExist").ConfigureAwait(false);

            Assert.False(response);
        }

        [Fact]
        public async Task DeleteReturnsTrueIfSuccessful()
        {
            // Arrange
            var institution = GetInstitution();
            var sut = GetInstitutionStore(seed: institution);
            _ = GetParticipantStore();

            // Act
            var response = await sut.DeleteById(institution.Id).ConfigureAwait(false);

            // Assert
            Assert.True(response);

            var found = await sut.GetById(institution.Id).ConfigureAwait(false);
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteFailsIfParticipantsExistForInstitution()
        {
            // Arrange

            var institution = GetInstitution(name: "Test institution");

            var participant = new ParticipantDto
            {
                Id = "234",
                PartitionKey = "participant::234",
                Name = "Test participant",
                Host = "https://participant:8080",
                InstitutionId = institution.Id,
                Status = ParticipantStatusDto.Active,
                Type = ParticipantTypeDto.Chatbot
            };

            var particpantStore = GetParticipantStore(participant);
            var sut = GetInstitutionStore(seed: institution);

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ParticipantDto>>(
                    async () => await sut.DeleteById(institution.Id).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetAllReturnsAllStoredInstitutions()
        {
            // Arrange
            var institution1 = GetInstitution(id: "234", name: "Test1");
            var institution2 = GetInstitution(id: "345", name: "Test2");

            var sut = GetInstitutionStore(institution1, institution2);

            // Act
            var response = await sut.GetAll().ConfigureAwait(false);

            // Assert
            _ = response.Should().BeEquivalentTo(new[] { institution1, institution2 });
        }

        [Fact]
        public virtual async Task UpdateCanUpdateAnInstitution()
        {
            // Arrange
            var institution = GetInstitution(name: "Test1");
            var sut = GetInstitutionStore(seed: institution);

            SetupInstitutionQuery(m => m.Id == institution.Id && m.Name == "Test2");

            // Act
            var institutionWithUpdates = GetInstitution(id: institution.Id, name: "Test2", status: InstitutionStatusDto.Disabled);
            var updatedInstitution = await sut.Update(institutionWithUpdates).ConfigureAwait(false);

            Assert.Equal(institutionWithUpdates.Id, updatedInstitution.Id);
            Assert.Equal(institutionWithUpdates.Name, updatedInstitution.Name);
            Assert.Equal(institutionWithUpdates.Status, updatedInstitution.Status);
        }

        [Fact]
        public async Task UpdateThrowsForNullModel()
        {
            // Arrange
            var sut = GetInstitutionStore();

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
            var sut = GetInstitutionStore();

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
            var sut = GetInstitutionStore();

            var institution = new InstitutionDto { Name = "Name", Status = InstitutionStatusDto.Disabled, Id = null };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(institution).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public virtual async Task UpdateThrowsForNonexistentInstitution()
        {
            // Arrange
            var sut = GetInstitutionStore();

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelNotFoundException<InstitutionDto>>(
                    () =>
                    {
                        var institution = GetInstitution(
                            id: "DoesntExist",
                            name: "Test1",
                            status: InstitutionStatusDto.Active);
                        return sut.Update(institution);
                    })
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForDuplicateName()
        {
            // Arrange
            var institution1 = GetInstitution(id: "123", name: "Test1");
            var institution2 = GetInstitution(id: "234", name: "Test2");
            var sut = GetInstitutionStore(institution1, institution2);

            var institutionWithUpdates =
                new InstitutionDto
                {
                    Id = institution2.Id,
                    Name = institution1.Name,
                    Status = institution2.Status
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<InstitutionDto>>(
                    async () => await sut.Update(institutionWithUpdates).ConfigureAwait(false))
                .ConfigureAwait(false);
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
                Status = status
            };
        }
    }
}
