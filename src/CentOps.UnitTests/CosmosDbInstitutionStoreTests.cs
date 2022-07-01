using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.Azure.Cosmos;
using Moq;
using System.Net;

namespace CentOps.UnitTests
{
    public class CosmosDbInstitutionStoreTests : BaseInstitutionStoreTests
    {
        private readonly Mock<Container> mockContainer = new();
        private readonly Mock<CosmosClient> mockClient = new();

        private readonly CosmosDbService sut;

        public CosmosDbInstitutionStoreTests()
        {
            mockContainer = new Mock<Container>();
            mockClient = new Mock<CosmosClient>();

            _ = mockClient
                .Setup(x => x.GetContainer(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(mockContainer.Object);

            sut = new CosmosDbService(mockClient.Object, It.IsAny<string>(), It.IsAny<string>());

            SetupDefaultReadItemStream();
        }

        protected override IInstitutionStore GetInstitutionStore(params InstitutionDto[] seedInstitutions)
        {
            var seedInstitutionList = seedInstitutions.ToList() ?? new List<InstitutionDto>();

            SetupCreateItem<InstitutionDto>();
            SetupFeedIterator(seedInstitutionList);
            SetupReadItemStream(seedInstitutionList);
            SetupReadItem(seedInstitutionList);
            SetupDeleteItem(seedInstitutionList);
            SetupUpdateItem(seedInstitutionList);

            return sut;
        }

        protected override IParticipantStore GetParticipantStore(params ParticipantDto[] seedParticipants)
        {
            var seedParticipantsList = seedParticipants.ToList() ?? new List<ParticipantDto>();

            SetupCreateItem<ParticipantDto>();
            SetupFeedIterator(seedParticipantsList);
            SetupReadItemStream(seedParticipantsList);
            SetupReadItem(seedParticipantsList);
            SetupDeleteItem(seedParticipantsList);
            SetupUpdateItem(seedParticipantsList);

            return sut;
        }

        private void SetupCreateItem<TModel>()
            where TModel : class, IModel
        {
            var mockItemResponse = new Mock<ItemResponse<TModel>>();

            _ = mockContainer
                .Setup(x =>
                    x.CreateItemAsync(
                        It.IsAny<TModel>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        default))
                .ReturnsAsync(mockItemResponse.Object);
        }

        private void SetupFeedIterator<TModel>(IList<TModel> models)
            where TModel : class, IModel
        {
            _ = mockContainer
                .Setup(c => c.GetItemQueryIterator<TModel>(
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()))
                .Returns<QueryDefinition, string, QueryRequestOptions>((query, continuation, options) =>
                {
                    var feedIteratorMock = new Mock<FeedIterator<TModel>>();

                    var hasMoreResultsSeq = feedIteratorMock.SetupSequence(f => f.HasMoreResults);
                    var readNextSeq = feedIteratorMock.SetupSequence(f => f.ReadNextAsync(It.IsAny<CancellationToken>()));

                    for (var i = 0; i < models.Count; i++)
                    {
                        hasMoreResultsSeq = hasMoreResultsSeq.Returns(true);

                        var model = models[i];

                        var enumerator = new List<TModel>() { model }.GetEnumerator();
                        var feedResponseMock = new Mock<FeedResponse<TModel>>();
                        _ = feedResponseMock.Setup(x => x.GetEnumerator()).Returns(enumerator);

                        readNextSeq = readNextSeq.ReturnsAsync(feedResponseMock.Object);
                    }

                    hasMoreResultsSeq = hasMoreResultsSeq.Returns(false);

                    return feedIteratorMock.Object;
                });
        }

        private void SetupDefaultReadItemStream()
        {
            // This setup is for the "not found" case. This will be ignored after the first call so it is safe to call multiple times.
            _ = mockContainer
                .Setup(m =>
                    m.ReadItemStreamAsync(
                        It.IsAny<string>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        default))
                .Returns<string, PartitionKey, ItemRequestOptions, CancellationToken>((id, pk, options, _) =>
                {
                    return Task.FromResult(new ResponseMessage(HttpStatusCode.NotFound));
                });
        }

        private void SetupReadItemStream<TModel>(IList<TModel> models)
            where TModel : class, IModel
        {
            foreach (var model in models)
            {
                _ = mockContainer
                    .Setup(m =>
                        m.ReadItemStreamAsync(
                            model.Id,
                            new PartitionKey(model.PartitionKey),
                            It.IsAny<ItemRequestOptions>(),
                            default))
                    .Returns<string, PartitionKey, ItemRequestOptions, CancellationToken>((id, pk, options, _) =>
                    {
                        var item = models
                            .FirstOrDefault(m => m.Id == id && new PartitionKey(m.PartitionKey) == pk);

                        var statusCode = item == null ? HttpStatusCode.NotFound : HttpStatusCode.OK;

                        return Task.FromResult(new ResponseMessage(statusCode));
                    });
            }
        }

        private void SetupReadItem<TModel>(IList<TModel> models)
            where TModel : class, IModel
        {
            _ = mockContainer
                .Setup(m =>
                    m.ReadItemAsync<TModel>(
                        It.IsAny<string>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()))
                .Returns<string, PartitionKey, ItemRequestOptions, CancellationToken>((id, pk, options, ct) =>
                {
                    var response = new Mock<ItemResponse<TModel>>();

                    var item = models.FirstOrDefault(m => m.Id == id && new PartitionKey(m.PartitionKey) == pk);

                    if (item != null)
                    {
                        _ = response.Setup(m => m.Resource).Returns(item);
                    }

                    return Task.FromResult(response.Object);
                });
        }

        private void SetupDeleteItem<TModel>(IList<TModel> models)
            where TModel : class, IModel
        {
            _ = mockContainer
                .Setup(m =>
                    m.DeleteItemAsync<TModel>(
                        It.IsAny<string>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()))
                .Returns<string, PartitionKey, ItemRequestOptions, CancellationToken>((id, pk, options, ct) =>
                {
                    var response = new Mock<ItemResponse<TModel>>();

                    var item = models.FirstOrDefault(m => m.Id == id && new PartitionKey(m.PartitionKey) == pk);
                    _ = models.Remove(item);

                    return Task.FromResult(response.Object);
                });
        }

        private void SetupUpdateItem<TModel>(IList<TModel> models)
            where TModel : class, IModel
        {
            _ = mockContainer
                .Setup(m =>
                    m.UpsertItemAsync(
                        It.IsAny<TModel>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()))
                .Returns<TModel, PartitionKey, ItemRequestOptions, CancellationToken>((incomingModel, pk, options, ct) =>
                {
                    var response = new Mock<ItemResponse<TModel>>();

                    var item = models.FirstOrDefault(m => m.Id == incomingModel.Id && new PartitionKey(m.PartitionKey) == pk);
                    _ = models.Remove(item);
                    models.Add(incomingModel);

                    _ = response.Setup(m => m.StatusCode).Returns(item == null ? HttpStatusCode.NotFound : HttpStatusCode.OK);

                    return Task.FromResult(response.Object);
                });
        }
    }
}
