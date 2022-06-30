using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;

namespace CentOps.UnitTests
{
    //public abstract class BaseStoreTests<TModelStore, TModel>
    //    where TModelStore : class, IModelStore<TModel>
    //    where TModel : class, IModel
    //{
    //    protected abstract TModelStore GetStore(params TModel[] seedInstitutions);
    //}

    public abstract class BaseInstitutionStoreTests// : BaseStoreTests<IInstitutionStore, InstitutionDto>
    {
        protected abstract IInstitutionStore GetStore(params InstitutionDto[] seedInstitutions);

        [Fact]
        public virtual async Task CreateCanStoreInstitution()
        {
            // Arrange
            var sut = GetStore();

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
            var sut = GetStore();

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
            var sut = GetStore();

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
            var sut = GetStore(new InstitutionDto { Name = "Test" });

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<InstitutionDto>>(
                    () =>
                    {
                        var institution2 = new InstitutionDto { Name = "Test", Status = InstitutionStatusDto.Active };
                        return sut.Create(institution2);
                    })
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteThrowsIfIdIsNull()
        {
            // Arrange
            var sut = GetStore();

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
            var sut = GetStore();

            // Act & Assert
            var response = await sut.DeleteById("DoesntExist").ConfigureAwait(false);

            Assert.False(response);
        }

        [Fact]
        public async Task DeleteReturnsTrueIfSuccessful()
        {
            // Arrange
            var sut = GetStore();

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
            var sut = GetStore();

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
            var sut = GetStore();

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
        public async Task UpdateCanUpdateAnInsitution()
        {
            // Arrange
            var sut = GetStore();

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
            var sut = GetStore();

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
            var sut = GetStore();

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
            var sut = GetStore();

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
            var sut = GetStore();

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
            var sut = GetStore();

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
