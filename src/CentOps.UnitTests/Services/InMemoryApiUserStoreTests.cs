using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;

namespace CentOps.UnitTests.Services
{
    public class InMemoryApiUserStoreTests
    {
        [Fact]
        public async Task CreateCanStoreApiUser()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser = new ApiUserDto
            {
                Name = "Test",
            };

            // Act
            var createdApiUser = await sut.Create(apiUser).ConfigureAwait(false);

            // Assert
            Assert.NotNull(createdApiUser.Id);
            Assert.Equal(apiUser.Name, createdApiUser.Name);
        }

        [Fact]
        public async Task CreateThrowsIfApiUsertIsNull()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentNullException>(
                    async () => await sut.Create(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfApiUserNameIsNull()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser = new ApiUserDto { Name = null };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Create(apiUser).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateThrowsIfDuplicateNameSpecified()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser1 = new ApiUserDto { Name = "Test" };
            _ = await sut.Create(apiUser1).ConfigureAwait(false);

            var apiUser2 = new ApiUserDto { Name = apiUser1.Name };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ApiUserDto>>(
                    async () => await sut.Create(apiUser2).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteThrowsIfIdIsNull()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.DeleteById(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteReturnsFalseIfApiUserDoesntExist()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            // Act & Assert
            var response = await sut.DeleteById("DoesntExist").ConfigureAwait(false);

            Assert.False(response);
        }

        [Fact]
        public async Task DeleteReturnsTrueIfSuccessful()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser = new ApiUserDto { Name = "Test" };

            var created = await sut.Create(apiUser).ConfigureAwait(false);

            // Act
            var response = await sut.DeleteById(created.Id).ConfigureAwait(false);

            // Assert
            Assert.True(response);

            var found = await sut.GetById(created.Id).ConfigureAwait(false);
            Assert.Null(found);
        }

        [Fact]
        public async Task GetAllReturnsAllStoredInsitutions()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser1 = new ApiUserDto { Name = "Test1" };
            _ = await sut.Create(apiUser1).ConfigureAwait(false);

            var apiUser2 = new ApiUserDto { Name = "Test2" };
            _ = await sut.Create(apiUser2).ConfigureAwait(false);

            // Act
            var response = await sut.GetAll().ConfigureAwait(false);

            // Assert
            _ = response.Should().BeEquivalentTo(new[] { apiUser1, apiUser2 });
        }

        [Fact]
        public async Task UpdateCanUpdateAnInsitution()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser = new ApiUserDto { Name = "Test1" };
            var createdApiUser = await sut.Create(apiUser).ConfigureAwait(false);

            // Act
            var apiUserWithUpdates = new ApiUserDto { Name = "Test2", Id = createdApiUser.Id };
            var updatedApiUser = await sut.Update(apiUserWithUpdates).ConfigureAwait(false);

            Assert.Equal(apiUserWithUpdates.Id, updatedApiUser.Id);
            Assert.Equal(apiUserWithUpdates.Name, updatedApiUser.Name);
        }

        [Fact]
        public async Task UpdateThrowsForNullModel()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentNullException>(
                    async () => await sut.Update(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForNullApiUserName()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser = new ApiUserDto { Name = null, Id = "1" };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(apiUser).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForNullInsitutionId()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser = new ApiUserDto { Name = "Name", Id = null };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(apiUser).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForNonexistentApiUser()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser = new ApiUserDto { Name = "Test1", Id = "DoesntExist" };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelNotFoundException<ApiUserDto>>(
                    async () => await sut.Update(apiUser).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateThrowsForDuplicateName()
        {
            // Arrange
            var sut = new InMemoryStore() as IApiUserStore;

            var apiUser1 = new ApiUserDto { Name = "Test1" };
            var createdApiUser1 = await sut.Create(apiUser1).ConfigureAwait(false);

            var apiUser2 = new ApiUserDto { Name = "Test2" };
            var createdApiUser2 = await sut.Create(apiUser2).ConfigureAwait(false);

            var apiUserWithUpdates =
                new ApiUserDto
                {
                    Name = apiUser1.Name,
                    Id = createdApiUser2.Id
                };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<ApiUserDto>>(
                    async () => await sut.Update(apiUserWithUpdates).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
