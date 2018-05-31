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
    ///     1. CannotBuyTokensIfTokenSaleNotStarted => if token sale has not been started, you should not be able to buy any tokens
    ///     2. CanBuyTokensIfPreSaleTokenSaleStarted => if token pre-sale has started, you can buy tokens
    ///     3. CanBuyTokensIfSoftCapSaleTokenSaleStarted => If token sale has started (soft cap) you can buy tokens
    ///     4. CanBuyTokensIfHardCapSaleTokenSaleStarted => If token sale has started (hard cap) you can buy tokens
    ///     5. CannotBuyMoreTokensThanSupply => You cannot buy more than the overall cap of tokens, even if you whish
    ///     6. CannotBuyNegativeAmountOfTokens => You cannot buy a negative amount of tokens
    ///     7. CannotBuyMoreThanPreSaleCapDuringPreSaleTest => You cannot buy more tokens than we have during a pre-sale
    ///     8. CorrectOverflowFromSoftCapToHardCapRateTest => The buying amount between soft cap to hard cap is correctly integrated
    ///     9. CorrectWalletSpreadPreSale => Wallets receive the correct contribution amount based on their allocation
    ///     10. CorrectWalletSpreadSoftCapSale => Wallets receive the correct contribution amount based on their allocation
    ///     11. CorrectWalletSpreadHardCapSale => Wallets receive the correct contribution amount based on their allocation
    ///     12. CorrectOverflowAndRefundTest =>
    ///     13. CanOnlyWithdrawOnce =>
    ///     14. CorrectHardCapSpreadWithAffiliateAndRefund =>
    ///     15. CanBuyForAWhitelistedAddressDelegated =>
    ///     16. CanBuyMultipleContributors =>
    ///     17. WeiRaisedIsCorrect =>
    ///     18. CorrectAmountFractionalAmount =>
    /// </summary>
    /// <seealso cref="test.BaseTest" />
    public class BuyTests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize BuyTests
        /// </summary>
        /// <param name="output"></param>
        public BuyTests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Private Fields

        /// <summary>
        /// The company percentage
        /// </summary>
        private const int CompanyPercentage = 50;

        /// <summary>
        /// The github bounty percentage
        /// </summary>
        private const int GithubBountyPercentage = 2;

        /// <summary>
        /// The icon bounty percentage
        /// </summary>
        private const int ICOBountyPercentage = 3;

        /// <summary>
        /// The mining pool percentage
        /// </summary>
        private const int MiningPoolPercentage = 15;

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public async Task CanBuyTokensIfHardCapSaleTokenSaleStarted()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CanBuyTokensIfPreSaleTokenSaleStarted()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CanBuyTokensIfSoftCapSaleTokenSaleStarted()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotBuyMoreThanPreSaleCapDuringPreSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotBuyMoreTokensThanSupply()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotBuyNegativeAmountOfTokens()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotBuyTokensIfTokenSaleNotStarted()
        {
            //Arrange
            Initialize();

            //Act
            Func<Task> exceptionAction = async () => await BuyTokens(AccountDictionary.ElementAt(0).Key, AccountDictionary.ElementAt(0).Key, GetEther(1));

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
        }

        [Fact]
        public async Task CorrectOverflowAndRefundTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CanOnlyWithdrawOnce()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
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
                })
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Initial withdrawal
            var releaseTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("withdrawAllTokens"),
                AccountDictionary.ElementAt(0).Key);

            //Act
            Func<Task> exceptionAction = async () => await ExecuteFunc(CrowdSaleContract.GetFunction("withdrawAllTokens"),
                AccountDictionary.ElementAt(0).Key);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
        }

        [Fact]
        public async Task CorrectHardCapSpreadWithAffiliateAndRefund()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
            string investorAddress = AccountDictionary.Last().Key;
            var amountContributed = GetEther(1);

            //Act
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
                {
                    x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                    x.ClosingTime = ConvertToUnixTimestamp(closingtime);
                    x.HardCapRate = 9259259259;
                    x.SoftCapRate = 9259259259;
                    x.HardCap = GetEther(1);
                })
                .WithContributors(new Dictionary<string, BigInteger>
                {
                    {investorAddress, amountContributed}
                })
                .WaitBeforeContributing(x => DateTime.UtcNow > openingtime)
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Act (release token balances)
            var releaseTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("withdrawAllTokens"),
                AccountDictionary.ElementAt(0).Key);

            //Assert
        }

        [Fact]
        public async Task CanBuyForAWhitelistedAddressDelegated()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task CanBuyMultipleContributors()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
            var contributors = new Dictionary<string, BigInteger>
            {
                {AccountDictionary.ElementAt(1).Key, GetEther(1)},
                {AccountDictionary.ElementAt(2).Key, GetEther(2)},
                {AccountDictionary.ElementAt(3).Key, GetEther(1.1231)},
                {AccountDictionary.ElementAt(4).Key, GetEther(0.01)}
            };
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
                {
                    x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                    x.ClosingTime = ConvertToUnixTimestamp(closingtime);
                })
                .WithContributors(contributors)
                .Prepare(async x =>
                {
                    //Open pre-sale
                    await OpenPreSale(AccountDictionary.ElementAt(0).Key);
                })
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Act (release token balances)
            var releaseTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("withdrawAllTokens"),
                AccountDictionary.ElementAt(0).Key);

            //Check contributors
            foreach (var contributor in contributors)
                (await GetBalance(contributor.Key)).Should().Be(contributor.Value * CrowdsaleConstructorModel.PreSaleRate);
        }

        [Fact]
        public async Task WeiRaisedIsCorrect()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
            var contributors = new Dictionary<string, BigInteger>
            {
                {AccountDictionary.ElementAt(1).Key, GetEther(1)},
                {AccountDictionary.ElementAt(2).Key, GetEther(2)},
                {AccountDictionary.ElementAt(3).Key, GetEther(1.1231)},
                {AccountDictionary.ElementAt(4).Key, GetEther(0.01)}
            };
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
                {
                    x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                    x.ClosingTime = ConvertToUnixTimestamp(closingtime);
                })
                .WithContributors(contributors)
                .Prepare(async x =>
                {
                    //Open pre-sale
                    await OpenPreSale(AccountDictionary.ElementAt(0).Key);
                })
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Act
            throw new NotImplementedException();
        }

        [Fact]
        public async Task CorrectAmountFractionalAmount()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
            string investorAddress = AccountDictionary.Last().Key;
            var amountContributed = GetEther(.015115154872);
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

            //Assert
            var investorAmount = amountContributed * CrowdsaleConstructorModel.PreSaleRate;
            (await GetAllocatedBalance(investorAddress)).Should().Be(0);

            //Spread check
            var balance = await GetBalance(investorAddress);
            balance.Should().Be(investorAmount);
            GetFractionalAmountOfTokens(balance).Should().BeApproximately(.015115154872 * (int)CrowdsaleConstructorModel.PreSaleRate, 0.00001);
        }

        [Fact]
        public async Task CorrectWalletSpreadPreSale()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
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
                })
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Act (release token balances)
            var releaseTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("withdrawAllTokens"),
                AccountDictionary.ElementAt(0).Key);

            //Assert
            var investorAmount = amountContributed * CrowdsaleConstructorModel.PreSaleRate;
            var allocationKey = investorAmount * 100 / 30;
            var companyAmount = allocationKey * new BigInteger(CompanyPercentage) / new BigInteger(100);
            var miningPoolAmount = allocationKey * new BigInteger(MiningPoolPercentage) / new BigInteger(100);
            var icoBountyAmount = allocationKey * new BigInteger(ICOBountyPercentage) / new BigInteger(100);
            var gitHubBountyAmount = allocationKey * new BigInteger(GithubBountyPercentage) / new BigInteger(100);
            (await GetAllocatedBalance(investorAddress)).Should().Be(0);

            //Spread check
            (await GetBalance(investorAddress)).Should().Be(investorAmount);
            (await GetBalance(CrowdsaleConstructorModel.CompanyReserve)).Should().Be(companyAmount);
            (await GetBalance(CrowdsaleConstructorModel.MiningPool)).Should().Be(miningPoolAmount);
            (await GetBalance(CrowdsaleConstructorModel.ICOBounty)).Should().Be(icoBountyAmount);
            (await GetBalance(CrowdsaleConstructorModel.GitHubBounty)).Should().Be(gitHubBountyAmount);

            //General check
            (companyAmount > investorAmount && investorAmount > miningPoolAmount &&
             miningPoolAmount > icoBountyAmount && icoBountyAmount > gitHubBountyAmount).Should().BeTrue();
            companyAmount.Should().BeGreaterThan(0);
            investorAmount.Should().BeGreaterThan(0);
            miningPoolAmount.Should().BeGreaterThan(0);
            icoBountyAmount.Should().BeGreaterThan(0);
            gitHubBountyAmount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CorrectWalletSpreadSoftCapSale()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            var closingtime = DateTime.UtcNow.Add(OpeningTimeDelay * 2);
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
                .WaitBeforeContributing(x => DateTime.UtcNow < openingtime.Add(OpeningTimeDelay))
                .WaitAfterContributing(x => DateTime.UtcNow < closingtime.Add(OpeningTimeDelay)));

            //Act (release token balances)
            var releaseTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("withdrawAllTokens"),
                AccountDictionary.ElementAt(0).Key);

            //Assert
            var investorAmount = amountContributed * CrowdsaleConstructorModel.SoftCapRate;
            var allocationKey = investorAmount * 100 / 30;
            var companyAmount = allocationKey * new BigInteger(CompanyPercentage) / new BigInteger(100);
            var miningPoolAmount = allocationKey * new BigInteger(MiningPoolPercentage) / new BigInteger(100);
            var icoBountyAmount = allocationKey * new BigInteger(ICOBountyPercentage) / new BigInteger(100);
            var gitHubBountyAmount = allocationKey * new BigInteger(GithubBountyPercentage) / new BigInteger(100);
            (await GetAllocatedBalance(investorAddress)).Should().Be(0);

            //Spread check
            (await GetBalance(investorAddress)).Should().Be(investorAmount);
            (await GetBalance(CrowdsaleConstructorModel.CompanyReserve)).Should().Be(companyAmount);
            (await GetBalance(CrowdsaleConstructorModel.MiningPool)).Should().Be(miningPoolAmount);
            (await GetBalance(CrowdsaleConstructorModel.ICOBounty)).Should().Be(icoBountyAmount);
            (await GetBalance(CrowdsaleConstructorModel.GitHubBounty)).Should().Be(gitHubBountyAmount);

            //General check
            (companyAmount > investorAmount && investorAmount > miningPoolAmount &&
             miningPoolAmount > icoBountyAmount && icoBountyAmount > gitHubBountyAmount).Should().BeTrue();
            companyAmount.Should().BeGreaterThan(0);
            investorAmount.Should().BeGreaterThan(0);
            miningPoolAmount.Should().BeGreaterThan(0);
            icoBountyAmount.Should().BeGreaterThan(0);
            gitHubBountyAmount.Should().BeGreaterThan(0);
        }

        #endregion Public Methods
    }
}