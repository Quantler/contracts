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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using test.Model;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. BuyerHasNoTokensDuringSaleOnlyAllocatedBalance => In the token contract, no one has tokens if the sale has not finished yet
    ///     2. CanTransferTokensFromWalletToOtherWalletAfterRelase => After tokens have been released, token holders can send tokens to other token holders
    /// </summary>
    /// <seealso cref="test.BaseTest" />
    public class TransferTests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize TransferTests
        /// </summary>
        /// <param name="output"></param>
        public TransferTests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task BuyerHasNoTokensDuringSaleOnlyAllocatedBalance()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.AddDays(2);
            string investorAddress = AccountDictionary.Last().Key;
            var amountContributed = GetEther(1);
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                x.ClosingTime = ConvertToUnixTimestamp(closingtime);
            })
                .WithContributors(new Dictionary<string, BigInteger>
                {
                    {investorAddress, amountContributed}
                })
                .Prepare(async x =>
                {
                    //Open pre-sale
                    await OpenPreSale(AccountDictionary.ElementAt(0).Key);
                }));

            //Act
            var tokenAllocatedBalance = (int)(await GetAllocatedBalance(investorAddress));
            var tokenBalance = (int)(await GetBalance(investorAddress));

            //Assert
            tokenAllocatedBalance.Should().BePositive();
            tokenBalance.Should().Be(0);
        }

        [Fact]
        public async Task CanTransferTokensFromWalletToOtherWalletAfterRelase()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
            string investorAddress = AccountDictionary.Last().Key;
            string recipientAddress = await CreateNewEthereumAddressAsync(Password);
            var amountContributed = GetEther(1);
            decimal fractionalAmount = 0.123456789123456789m;
            BigInteger Amount = new BigInteger(123456789123456789);

            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                x.ClosingTime = ConvertToUnixTimestamp(closingtime);
            })
                .WithContributors(new Dictionary<string, BigInteger>
                {
                    {investorAddress, amountContributed}
                })
                .Prepare(async x =>
                {
                    //Open pre-sale
                    await OpenPreSale(AccountDictionary.ElementAt(0).Key);
                })
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Act (release token balances)
            var releaseTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("withdrawAllTokens"),
                AccountDictionary.ElementAt(0).Key);

            var investorTokens = await GetBalance(investorAddress);

            //Act send tokens to another wallet
            var approve = await ExecuteFunc(TokenContract.GetFunction("approve"), investorAddress, recipientAddress,
                Amount);
            var send = await ExecuteFunc(TokenContract.GetFunction("transferFrom"), investorAddress, recipientAddress,
                Amount);

            //Assert
            (await GetBalance(investorAddress)).Should().Be(investorTokens - Amount);
            (await GetBalance(recipientAddress)).Should().Be(Amount);
        }

        #endregion Public Methods
    }
}