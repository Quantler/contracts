using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. CanDeployCorrectContract => base test for deploying the tokensale contract and the crowdsale contract
    /// </summary>
    public class DeployTests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize DeployTests
        /// </summary>
        /// <param name="output"></param>
        public DeployTests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Public Methods

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

        #endregion Public Methods
    }
}