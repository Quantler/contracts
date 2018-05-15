using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. OwnerCanMintTokensDuringSaleTest => Owner has the ability to mint tokens during sale
    ///     2. OwnerCannotMintTokensAfterSaleTest => Owner cannot mint tokens after the sale has been concluded
    ///     3. NonOwnerCannotMintTokensDuringSaleTest => If you are not an owner, you cannot mint
    ///     4. NonOwnerCannotMintTokensAfterSaleTest => If you are not an owner, you cannot mint
    /// </summary>
    /// <seealso cref="test.BaseTest" />
    public class MintTests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize MintTests
        /// </summary>
        /// <param name="output"></param>
        public MintTests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task NonOwnerCannotMintTokensAfterSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task NonOwnerCannotMintTokensDuringSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task OwnerCanMintTokensDuringSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task OwnerCannotMintTokensAfterSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        #endregion Public Methods
    }
}