#region License

/*
 *  Copyright 2018 Quantler B.V.
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 *  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 *  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *	The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 *  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
*/

#endregion License

using FluentAssertions;
using Nethereum.JsonRpc.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using test.Model;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. ContractOwnerIsCrowdSaleContract => Owner is the crowdsale contract for minting after the crowdsale
    ///     2. NonOwnerCannotMintTokensDuringSaleTest => If you are not an owner, you cannot mint
    ///     3. NonOwnerCannotMintTokensAfterSaleTest => If you are not an owner, you cannot mint
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
        public async Task ContractOwnerIsCrowdSaleContract()
        {
            //Arrange
            Initialize();

            //Act
            var owner = await TokenContract.GetFunction("owner").CallAsync<string>();

            //Assert
            owner.Should().Be(CrowdSaleContract.Address);
        }

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

        #endregion Public Methods
    }
}