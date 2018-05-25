using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.JsonRpc.Client;
using test.Model;
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
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
                {
                    x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                    x.ClosingTime = ConvertToUnixTimestamp(closingtime);
                })
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Act
            Func<Task> exceptionAction = async () => await ExecuteFunc(TokenContract.GetFunction("mint"), AccountDictionary.ElementAt(0).Key, 1200);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
        }

        [Fact]
        public async Task NonOwnerCannotMintTokensDuringSaleTest()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
                {
                    x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                })
                .WaitAfterContributing(x => DateTime.UtcNow < openingtime.Add(OpeningTimeDelay)));

            //Act
            Func<Task> exceptionAction = async () => await ExecuteFunc(TokenContract.GetFunction("mint"), AccountDictionary.ElementAt(0).Key, 1200);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
        }

        [Fact]
        public async Task ContractOwnerIsCrowdSaleContract()
        {
            //Arrange
            Initialize();

            //Act
            var owner = await TokenContract.GetFunction("owner").CallAsync<string>();

            //Assert
            owner.Should().Be(CrowdSaleContract.Address);
        }

        #endregion Public Methods
    }
}