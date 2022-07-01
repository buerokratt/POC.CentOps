using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using Microsoft.Azure.Cosmos;
using Moq;
using System.Linq.Expressions;

namespace CentOps.UnitTests
{
    public class ModelStoreBuilder<TModel>
        where TModel : class, IModel
    {
        private readonly Mock<Container> mockContainer;
        private readonly IList<TModel> models;

        public ModelStoreBuilder(Mock<Container> container, IList<TModel> seed)
        {
            mockContainer = container;
            models = seed;
        }

        public ModelStoreBuilder<TModel> SetupFeedIterator(Expression<Func<TModel, bool>> filter)
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

                    var filteredModels = models.Where(filter.Compile());

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

            return this;
        }
    }
}
