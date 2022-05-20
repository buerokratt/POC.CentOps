using CentOps.Api.Controllers;

namespace CentOps.UnitTests
{
    public class ValuesControllerTests
    {
        private readonly ValuesController sut;

        public ValuesControllerTests()
        {
            sut = new ValuesController();
        }

        [Fact]
        public void GetReturnsExpected()
        {
            // Arrange
            var expectedResult = new string[] { "value1", "value2" };

            // Act
            var result = sut.Get();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10000000)]
        public void GetByIdReturnsExpected(int id)
        {
            // Act
            var result = sut.Get(id);

            // Assert
            Assert.Equal(id, result);
        }
    }
}