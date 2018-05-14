using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    public class DeployTests: BaseTest
    {
        /// <summary>
        /// Initialize DeployTests
        /// </summary>
        /// <param name="output"></param>
        public DeployTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void CanDeployCorrectContract()
        {
            //Arrange


            //Act
            Initialize();

            //Assert
            TokenContract.Should().NotBeNull();
            TokenContract.Address.Should().NotBeNullOrWhiteSpace();
            CrowdSaleContract.Should().NotBeNull();
            CrowdSaleContract.Address.Should().NotBeNullOrWhiteSpace();
        }
    }
}
