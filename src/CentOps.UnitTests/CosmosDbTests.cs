using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;

namespace CentOps.UnitTests
{
    public class CosmosDbTests
    {
        [Fact]
        public async Task CreateCanStoreInstitution()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            var institution =
                new InstitutionDto
                {
                    Name = "Test",
                    Status = InstitutionStatusDto.Active
                };

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();
            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var myInstitutions = new List<InstitutionDto> { };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);


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
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var myInstitutions = new List<InstitutionDto> { };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

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
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var myInstitutions = new List<InstitutionDto> { };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

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
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var institutionName = "Test";
            var myInstitutions = new List<InstitutionDto> { new InstitutionDto { Name = institutionName, Status = InstitutionStatusDto.Active } };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            var institution2 = new InstitutionDto { Name = institutionName, Status = InstitutionStatusDto.Active };

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
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var myInstitutions = new List<InstitutionDto> { };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(false);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

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
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            // CreateItemAsync
            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var myInstitutions = new List<InstitutionDto> { };

            // GetItemQueryIterator
            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(false);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer.Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            // ReadItemStreamAsync
            var institution =
                new InstitutionDto
                {
                    Name = "Test",
                    Status = InstitutionStatusDto.Active
                };
            var partitionKey = new PartitionKey($"institution::{institution.Id}");
            var responseMsg = new ResponseMessage(System.Net.HttpStatusCode.NotFound);
            _ = mockContainer.Setup(x => x.ReadItemStreamAsync(It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .Returns(Task.FromResult(responseMsg));
            responseMsg.Dispose();

            // DeleteItemAsync

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            // Act & Assert
            var response = await sut.DeleteById("DoesntExist").ConfigureAwait(false);

            Assert.False(response);
        }

        [Fact]
        public async Task DeleteReturnsTrueIfSuccessful()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            // CreateItemAsync
            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var myInstitutions = new List<InstitutionDto> { };

            // GetItemQueryIterator
            var feedResponseMockInstitution = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMockInstitution.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMockInstitution = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMockInstitution.Setup(f => f.HasMoreResults).Returns(false);
            _ = feedIteratorMockInstitution
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMockInstitution.Object)
                .Callback(() => feedIteratorMockInstitution
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer.Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMockInstitution.Object);

            var myParticipants = new List<ParticipantDto> { };

            var feedResponseMockParticipant = new Mock<FeedResponse<ParticipantDto>>();
            _ = feedResponseMockParticipant.Setup(x => x.GetEnumerator()).Returns(myParticipants.GetEnumerator());

            var feedIteratorMockParticipant = new Mock<FeedIterator<ParticipantDto>>();
            _ = feedIteratorMockParticipant.Setup(f => f.HasMoreResults).Returns(false);
            _ = feedIteratorMockParticipant
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMockParticipant.Object)
                .Callback(() => feedIteratorMockParticipant
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer.Setup(c => c.GetItemQueryIterator<ParticipantDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMockParticipant.Object);

            // ReadItemStreamAsync
            var responseMsg = new ResponseMessage(System.Net.HttpStatusCode.OK);
            _ = mockContainer.Setup(x => x.ReadItemStreamAsync(It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .Returns(Task.FromResult(responseMsg));
            responseMsg.Dispose();

            // ReadItemAsync
            _ = mockContainer.Setup(x => x.ReadItemAsync<InstitutionDto>(It.IsAny<string>(), It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .Returns(Task.FromResult(mockItemResponse.Object));

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            var institution = new InstitutionDto { Name = "Test", Status = InstitutionStatusDto.Active };

            // Act
            var created = await sut.Create(institution).ConfigureAwait(false);
            var response = await sut.DeleteById(created.Id).ConfigureAwait(false);

            // Assert
            Assert.True(response);

            var found = await sut.GetById(institution.Id).ConfigureAwait(false);
            Assert.Null(found);
        }

        [Fact]
#pragma warning disable CA1506
        public async Task DeleteFailsIfParticipantsExistForInstitution()
#pragma warning restore CA1506
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();
            var institution = new InstitutionDto { Name = "Test", Status = InstitutionStatusDto.Active };

            // CreateItemAsync
            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            var myInstitutions = new List<InstitutionDto> { };

            // GetItemQueryIterator
            var feedResponseMockInstitution = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMockInstitution.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMockInstitution = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMockInstitution.Setup(f => f.HasMoreResults).Returns(false);
            _ = feedIteratorMockInstitution
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMockInstitution.Object)
                .Callback(() => feedIteratorMockInstitution
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer.Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMockInstitution.Object);


            var feedResponseMockParticipant = new Mock<FeedResponse<ParticipantDto>>();


            // ReadItemStreamAsync
            var responseMsg = new ResponseMessage(System.Net.HttpStatusCode.OK);
            _ = mockContainer.Setup(x => x.ReadItemStreamAsync(It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .Returns(Task.FromResult(responseMsg));
            responseMsg.Dispose();

            // ReadItemAsync
            _ = mockContainer.Setup(x => x.ReadItemAsync<InstitutionDto>(It.IsAny<string>(), It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .Returns(Task.FromResult(mockItemResponse.Object));

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            var createdInstitution = await sut.Create(institution).ConfigureAwait(false);
            _ = mockItemResponse.Setup(f => f.Resource).Returns(createdInstitution);

            var participant =
                new ParticipantDto
                {
                    Id = "123",
                    Name = "Test",
                    Host = "https://participant:8080",
                    InstitutionId = createdInstitution.Id,
                    Status = ParticipantStatusDto.Active,
                    Type = ParticipantTypeDto.Chatbot
                };

            var myParticipants = new List<ParticipantDto> { participant };
            var feedIteratorMockParticipant = new Mock<FeedIterator<ParticipantDto>>();
            _ = feedResponseMockParticipant.Setup(x => x.GetEnumerator()).Returns(myParticipants.GetEnumerator());
            _ = feedIteratorMockParticipant.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMockParticipant
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMockParticipant.Object)
                .Callback(() => feedIteratorMockParticipant
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer.Setup(c => c.GetItemQueryIterator<ParticipantDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMockParticipant.Object);

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
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);


            var institution1 = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active };
            var institution2 = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Active };
            var myInstitutions = new List<InstitutionDto> { institution1, institution2 };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            // 
            // _ = await sut.Create(institution1).ConfigureAwait(false);

            // 
            // _ = await sut.Create(institution2).ConfigureAwait(false);

            // Act
            var response = await sut.GetAll().ConfigureAwait(false);

            // Assert
            _ = response.Should().BeEquivalentTo(new[] { institution1, institution2 });
        }

        [Fact]
        public async Task UpdateCanUpdateAnInstitution()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);


            var createdInstitution = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active, Id = "id" };
            var myInstitutions = new List<InstitutionDto> { };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);


            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            // Act
            var institutionWithUpdates = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Disabled, Id = createdInstitution.Id };
            var mockResponse = new Mock<ItemResponse<InstitutionDto>>();
            _ = mockResponse.Setup(m => m.Resource).Returns(institutionWithUpdates);
            _ = mockContainer
                .Setup(c => c.ReplaceItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

            var updatedInstitution = await sut.Update(institutionWithUpdates).ConfigureAwait(false);

            Assert.Equal(institutionWithUpdates.Id, updatedInstitution.Id);
            Assert.Equal(institutionWithUpdates.Name, updatedInstitution.Name);
            Assert.Equal(institutionWithUpdates.Status, updatedInstitution.Status);
        }

        [Fact]
        public async Task UpdateThrowsForNullModel()
        {
            // Arrange
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);


            var createdInstitution = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active, Id = "id" };
            var myInstitutions = new List<InstitutionDto> { };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);


            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            // Act
            var institutionWithUpdates = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Disabled, Id = createdInstitution.Id };
            var mockResponse = new Mock<ItemResponse<InstitutionDto>>();
            _ = mockResponse.Setup(m => m.Resource).Returns(institutionWithUpdates);
            _ = mockContainer
                .Setup(c => c.ReplaceItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

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
            var mockContainer = new Mock<Container>();
            var mockClient = new Mock<CosmosClient>();

            _ = mockClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);


            var createdInstitution = new InstitutionDto { Name = "Test1", Status = InstitutionStatusDto.Active, Id = "id" };
            var myInstitutions = new List<InstitutionDto> { };

            var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
            _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(myInstitutions.GetEnumerator());

            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();
            _ = feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);
            _ = feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);


            var sut = new CosmosDbService(mockClient.Object, "", "") as IInstitutionStore;

            // Act
            var institutionWithUpdates = new InstitutionDto { Name = "Test2", Status = InstitutionStatusDto.Disabled, Id = createdInstitution.Id };
            var mockResponse = new Mock<ItemResponse<InstitutionDto>>();
            _ = mockResponse.Setup(m => m.Resource).Returns(institutionWithUpdates);
            _ = mockContainer
                .Setup(c => c.ReplaceItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

            var institution = new InstitutionDto { Name = null, Status = InstitutionStatusDto.Disabled, Id = "1" };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ArgumentException>(
                    async () => await sut.Update(institution).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
