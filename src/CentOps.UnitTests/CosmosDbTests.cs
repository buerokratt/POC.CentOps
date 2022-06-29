using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
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

            // var institution1 = new InstitutionDto { Name = "Test", Status = InstitutionStatusDto.Active };
            // _ = await sut.Create(institution1).ConfigureAwait(false);

            var institution2 = new InstitutionDto { Name = institutionName, Status = InstitutionStatusDto.Active };

            // Act & Assert
            _ = await Assert
                .ThrowsAsync<ModelExistsException<InstitutionDto>>(
                    async () => await sut.Create(institution2).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
