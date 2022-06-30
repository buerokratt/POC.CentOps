using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.Azure.Cosmos;
using Moq;

namespace CentOps.UnitTests
{
    public class CosmosDbInstitutionStoreTests : BaseInstitutionStoreTests
    {
        private readonly Mock<Container> mockContainer = new();
        private readonly Mock<CosmosClient> mockClient = new();

        public CosmosDbInstitutionStoreTests()
        {
            mockContainer = new Mock<Container>();
            mockClient = new Mock<CosmosClient>();

            _ = mockClient
                .Setup(x => x.GetContainer(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(mockContainer.Object);
        }

        public override Task CreateCanStoreInstitution()
        {
            // Arrange mocks for query response
            //_ = SetupFeedIterator();

            // Arrange mocks for create item response
            var mockItemResponse = new Mock<ItemResponse<InstitutionDto>>();

            _ = mockContainer.Setup(x => x.CreateItemAsync(It.IsAny<InstitutionDto>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockItemResponse.Object);

            return base.CreateCanStoreInstitution();
        }

        protected override IInstitutionStore GetStore(params InstitutionDto[] seedInstitutions)
        {
            if (seedInstitutions == null)
            {
                seedInstitutions = Array.Empty<InstitutionDto>();
            }

            SetupFeedIterator(seedInstitutions);

            return new CosmosDbService(mockClient.Object, It.IsAny<string>(), It.IsAny<string>());
        }

        private void SetupFeedIterator(params InstitutionDto[] institutions)
        {
            var feedIteratorMock = new Mock<FeedIterator<InstitutionDto>>();

            var hasMoreResultsSeq = feedIteratorMock.SetupSequence(f => f.HasMoreResults);
            var readNextSeq = feedIteratorMock.SetupSequence(f => f.ReadNextAsync(It.IsAny<CancellationToken>()));

            for (var i = 0; i < institutions.Length; i++)
            {
                hasMoreResultsSeq = hasMoreResultsSeq.Returns(true);

                var institution = institutions[i];

                var enumerator = new List<InstitutionDto>() { institution }.GetEnumerator();
                var feedResponseMock = new Mock<FeedResponse<InstitutionDto>>();
                _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(enumerator);

                readNextSeq = readNextSeq.ReturnsAsync(feedResponseMock.Object);
            }

            hasMoreResultsSeq = hasMoreResultsSeq.Returns(false);

            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<InstitutionDto>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);
        }
    }
}
