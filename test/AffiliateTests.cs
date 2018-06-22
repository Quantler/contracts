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
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using test.Model;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. OwnerCanAddAffiliateTest => Owner can add new affiliate links
    ///     2. NonOwnerCannotAddAffiliateTest => Non-Owners cannot add affiliate links
    ///     3. NonAffiliateDoesNotGetAdditionalTokens => Non-Affiliate does not get additional tokens
    ///     4. AffiliateGetCorrectTokenAmountPreSale => Correat allocation during the presale
    ///     5. AffiliateGetCorrectTokenAmountMainSoftCapSale => Correct allocation during the softcap
    ///     6. AffiliateGetCorrectTokenAmountMainHardCapSale => Correct allocation during the hardcap
    /// </summary>
    /// <seealso cref="test.BaseTest" />
    public class AffiliateTests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize AffiliateTests
        /// </summary>
        /// <param name="output"></param>
        public AffiliateTests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task AffiliateGetCorrectTokenAmountMainHardCapSale()
        {
            //Arrange
            var input = new CrowdsaleConstructorModel();
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            Initialize(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
                x.SofCap = 1;
            });
            string investorAddress = AccountDictionary.Last().Key;
            string affiliateAddress = await CreateNewEthereumAddressAsync(Password);
            var amountContributed = GetEther(1);

            //Wait for the tokensale to start
            while (DateTime.UtcNow < openingtime)
                Thread.Sleep(TimeSpan.FromSeconds(1));

            //Open to whitelist
            var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, investorAddress, amountContributed);
            whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, AccountDictionary.ElementAt(3).Key, amountContributed);

            //Get contract
            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), investorAddress);

            //Add affiliate
            var affiliateReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("addAffiliate"),
                AccountDictionary.ElementAt(0).Key, affiliateAddress, investorAddress);

            //Hit the softcap
            await BuyTokens(AccountDictionary.ElementAt(3).Key, AccountDictionary.ElementAt(3).Key, BigInteger.One, await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName),
                AccountDictionary.ElementAt(3).Key));

            //Act
            var tokenbuyTransactionRecepit = await BuyTokens(investorAddress, investorAddress, amountContributed, contractLinkBuyer);
            var resultInvestor = await GetAllocatedBalance(investorAddress);
            var resultAffiliate = await GetAllocatedBalance(affiliateAddress);

            //Assert
            var expectedInvestor = (amountContributed * input.HardCapRate) * new BigInteger(105) / new BigInteger(100);
            var expectedAffiliate = (amountContributed * input.HardCapRate) * new BigInteger(5) / new BigInteger(100);
            resultInvestor.Should().Be(expectedInvestor, $"Expected contribution of {expectedInvestor} for investor {investorAddress}");
            resultAffiliate.Should().Be(expectedAffiliate, $"Expected contribution of {expectedAffiliate} for affiliate {affiliateAddress}");
            expectedInvestor.Should().BeGreaterThan(amountContributed / input.HardCapRate, "You should get more tokens together than when you are alone");
        }

        [Fact]
        public async Task AffiliateGetCorrectTokenAmountMainSoftCapSale()
        {
            //Arrange
            var input = new CrowdsaleConstructorModel();
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            Initialize(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            });
            string investorAddress = AccountDictionary.Last().Key;
            string affiliateAddress = await CreateNewEthereumAddressAsync(Password);
            var amountContributed = GetEther(1);

            //Wait for the tokensale to start
            while (DateTime.UtcNow < openingtime)
                Thread.Sleep(TimeSpan.FromSeconds(1));

            //Open to whitelist
            var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, investorAddress, amountContributed);

            //Get contract
            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), investorAddress);

            //Add affiliate
            var affiliateReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("addAffiliate"),
                AccountDictionary.ElementAt(0).Key, affiliateAddress, investorAddress);

            //Act
            var tokenbuyTransactionRecepit = await BuyTokens(investorAddress, investorAddress, amountContributed, contractLinkBuyer);
            var resultInvestor = await GetAllocatedBalance(investorAddress);
            var resultAffiliate = await GetAllocatedBalance(affiliateAddress);

            //Assert
            var expectedInvestor = (amountContributed * input.SoftCapRate) * new BigInteger(105) / new BigInteger(100);
            var expectedAffiliate = (amountContributed * input.SoftCapRate) * new BigInteger(5) / new BigInteger(100);
            resultInvestor.Should().Be(expectedInvestor, $"Expected contribution of {expectedInvestor} for investor {investorAddress}");
            resultAffiliate.Should().Be(expectedAffiliate, $"Expected contribution of {expectedAffiliate} for affiliate {affiliateAddress}");
            expectedInvestor.Should().BeGreaterThan(amountContributed / input.SoftCapRate, "You should get more tokens together than when you are alone");
        }

        [Fact]
        public async Task AffiliateGetCorrectTokenAmountPreSale()
        {
            //Arrange
            var input = new CrowdsaleConstructorModel();
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            Initialize();
            string investorAddress = AccountDictionary.Last().Key;
            string affiliateAddress = await CreateNewEthereumAddressAsync(Password);
            var amountContributed = GetEther(1);

            //Open tokensale pre-sale
            await OpenPreSale(AccountDictionary.ElementAt(0).Key);

            //Open to whitelist
            var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, investorAddress, amountContributed);

            //Get contract
            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), investorAddress);

            //Add affiliate
            var affiliateReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("addAffiliate"),
                AccountDictionary.ElementAt(0).Key, affiliateAddress, investorAddress);

            //Act
            var tokenbuyTransactionRecepit = await BuyTokens(investorAddress, investorAddress, amountContributed, contractLinkBuyer);
            var resultInvestor = await GetAllocatedBalance(investorAddress);
            var resultAffiliate = await GetAllocatedBalance(affiliateAddress);

            //Assert
            var expectedInvestor = (amountContributed * input.PreSaleRate) * new BigInteger(105) / new BigInteger(100);
            var expectedAffiliate = (amountContributed * input.PreSaleRate) * new BigInteger(5) / new BigInteger(100);
            resultInvestor.Should().Be(expectedInvestor, $"Expected contribution of {expectedInvestor} for investor {investorAddress}");
            resultAffiliate.Should().Be(expectedAffiliate, $"Expected contribution of {expectedAffiliate} for affiliate {affiliateAddress}");
            expectedInvestor.Should().BeGreaterThan(amountContributed / input.PreSaleRate, "You should get more tokens together than when you are alone");
        }

        [Fact]
        public async Task NonAffiliateDoesNotGetAdditionalTokens()
        {
            //Arrange
            var input = new CrowdsaleConstructorModel();
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            Initialize();
            string address = AccountDictionary.Last().Key;
            var amountContributed = GetEther(1);

            //Open tokensale pre-sale
            await OpenPreSale(AccountDictionary.ElementAt(0).Key);

            var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, address, amountContributed);

            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), address);

            //Act
            var tokenbuyTransactionRecepit = await BuyTokens(address, address, amountContributed, contractLinkBuyer);
            var result = await GetAllocatedBalance(address);

            //Assert
            result.Should().Be(amountContributed * input.PreSaleRate, $"Expected contribution of {amountContributed / input.PreSaleRate}");
        }

        [Fact]
        public async Task NonOwnerCannotAddAffiliateTest()
        {
            //Arrange
            Initialize();
            var fromAddress = AccountDictionary.Last().Key;
            var contract = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), fromAddress);
            string investorAddress = await CreateNewEthereumAddressAsync(Password);
            string affiliateAddress = await CreateNewEthereumAddressAsync(Password);

            //Act
            Func<Task> exceptionAction = async () => await ExecuteFunc(contract.GetFunction("addAffiliate"),
                fromAddress, investorAddress, affiliateAddress);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
            var found = await GetCurrentgetAffiliate(fromAddress);
            found.Should().Be("0x0000000000000000000000000000000000000000", $"Expected {fromAddress} not to be on the affiliate list!");
        }

        [Fact]
        public async Task OwnerCanAddAffiliateTest()
        {
            //Arrange
            Initialize();
            string investorAddress = await CreateNewEthereumAddressAsync(Password);
            string affiliateAddress = await CreateNewEthereumAddressAsync(Password);

            //Act
            var result = await ExecuteFunc(CrowdSaleContract.GetFunction("addAffiliate"),
                AccountDictionary.ElementAt(0).Key, affiliateAddress, investorAddress);

            //Assert
            result.Status.HexValue.Should().Be("0x1");
            var found = await GetCurrentgetAffiliate(investorAddress);
            found.Should().Be(affiliateAddress, $"Expected {investorAddress} to be the affiliate of {affiliateAddress}!");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns the addresses currently on the whitelist.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCurrentgetAffiliate(string investorAddress) =>
            await CrowdSaleContract.GetFunction("getAffiliate").CallAsync<string>(investorAddress);

        #endregion Private Methods
    }
}