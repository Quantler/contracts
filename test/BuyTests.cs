using FluentAssertions;
using System;
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
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CorrectOverflowFromSoftCapToHardCapRateTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CorrectWalletSpreadHardCapSale()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CorrectWalletSpreadPreSale()
        {
            //Arrange
            var input = new CrowdsaleConstructorModel();
            Initialize();
            string investorAddress = AccountDictionary.Last().Key;
            var amountContributed = GetEther(1);

            //Open tokensale pre-sale
            await OpenPreSale(AccountDictionary.ElementAt(0).Key);

            //Open to whitelist
            var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, investorAddress, amountContributed);

            //Get contract
            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), investorAddress);

            //Act
            var tokenbuyTransactionRecepit = await BuyTokens(investorAddress, investorAddress, amountContributed, contractLinkBuyer);
            var resultInvestor = await GetAllocatedBalance(investorAddress);

            //Assert
            var investorAmount = amountContributed / input.PreSaleRate;
            var companyAmount = investorAmount * new BigInteger(CompanyPercentage) / new BigInteger(30);
            var miningPoolAmount = investorAmount * new BigInteger(MiningPoolPercentage) / new BigInteger(30);
            var icoBountyAmount = investorAmount * new BigInteger(ICOBountyPercentage) / new BigInteger(30);
            var gitHubBountyAmount = investorAmount * new BigInteger(GithubBountyPercentage) / new BigInteger(30);
            (await GetAllocatedBalance(investorAddress)).Should().Be(investorAmount);
            (await GetAllocatedBalance(input.CompanyReserve)).Should().Be(companyAmount);
            (await GetAllocatedBalance(input.MiningPool)).Should().Be(miningPoolAmount);
            (await GetAllocatedBalance(input.ICOBounty)).Should().Be(icoBountyAmount);
            (await GetAllocatedBalance(input.GitHubBounty)).Should().Be(gitHubBountyAmount);
            //General check
            (companyAmount > investorAmount && investorAmount > miningPoolAmount &&
             miningPoolAmount > icoBountyAmount && icoBountyAmount > gitHubBountyAmount).Should().BeTrue();
        }

        [Fact]
        public async Task CorrectWalletSpreadSoftCapSale()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        #endregion Public Methods
    }
}