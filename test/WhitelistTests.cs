using FluentAssertions;
using Nethereum.JsonRpc.Client;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. CorrectAmountRegisteredForWhitelist => Add a user to the whitelist and check if the correct amount is registered
    ///     2. UnknownWhitelistAddressHasNoAmount => Someone who is not added to the whitelist also has no whitelisted amount
    ///     3. UnknownWhitelistAddressHasNoContribution => Someone who is not added to the whitelist also has no max contribution amount
    ///     4. OwnerCanAddToWhitelistTest => Owner is allowed to add someone to the whitelist
    ///     5. StrangerCannotAddToWhitelistTest => Stranger is not allowed to add him or herself to the whitelist
    ///     6. CanAddMultipleWhitelistedAddressesFromAllowed => Allowed to add multiple addresses from an owner address
    ///     7. CannotAddMultipleWhitelistedAddressesFromDisallowed => Not allowed to add multiple addresses from a nonowner address
    ///     8. CannotBuyMoreThanAllowed => Whitelisted person cannot buy more than is allowed to contribute
    ///     9. CannotBuyAnythingWhenNotWhitelisted => Non-whitelisted person cannot buy any tokens when this person is not whitelisted
    /// </summary>
    /// <seealso cref="BaseTest" />
    public class WhitelistTests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize WhitelistTests
        /// </summary>
        /// <param name="output"></param>
        public WhitelistTests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task CanAddMultipleWhitelistedAddressesFromAllowed()
        {
            //Arrange
            Initialize();
            string[] inputAddress =
            {
                await CreateNewEthereumAddressAsync(Password),
                await CreateNewEthereumAddressAsync(Password),
                await CreateNewEthereumAddressAsync(Password),
                await CreateNewEthereumAddressAsync(Password)
            };
            var amount = GetEther(15);

            //Act
            var result = await ExecuteFunc(CrowdSaleContract.GetFunction("setGroupCap"),
                AccountDictionary.ElementAt(0).Key, inputAddress, amount);

            //Assert
            result.Status.HexValue.Should().Be("0x01");
            foreach (var address in inputAddress)
                (await GetCurrentWhitelistInfo(address)).Should().Be(amount, $"Expected {address} to be whitelisted!");
        }

        [Fact]
        public async Task CannotAddMultipleWhitelistedAddressesFromDisallowed()
        {
            //Arrange
            Initialize();
            string[] inputAddress =
            {
                await CreateNewEthereumAddressAsync(Password),
                await CreateNewEthereumAddressAsync(Password),
                await CreateNewEthereumAddressAsync(Password),
                await CreateNewEthereumAddressAsync(Password)
            };
            var amount = GetEther(15);
            var fromAddress = AccountDictionary.Last().Key;
            var contract = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), fromAddress);

            //Act
            Func<Task> exceptionAction = async () => await ExecuteFunc(contract.GetFunction("setGroupCap"),
                fromAddress, inputAddress, amount);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
            foreach (var address in inputAddress)
                (await GetCurrentWhitelistInfo(address)).Should().Be(0, $"Expected {address} not to be whitelisted!");
        }

        [Fact]
        public async Task CannotBuyAnythingWhenNotWhitelisted()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            Initialize(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            });
            string address = AccountDictionary.Last().Key;
            var amountWhitelisted = GetEther(0);
            var amountContributed = GetEther(5);

            //Wait for the tokensale to start
            while (DateTime.UtcNow < openingtime)
                Thread.Sleep(TimeSpan.FromSeconds(1));

            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), address);

            //Act
            Func<Task> exceptionAction = async () => await BuyTokens(address, address, amountContributed, contractLinkBuyer);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
            var found = await GetCurrentWhitelistInfo(address);
            var contributed = await GetCurrentWhitelistContribution(address);
            found.Should().Be(0, $"Expected {address} not to be whitelisted!");
            found.Should().Be(amountWhitelisted,
                $"Expected whitelisted amount for address {address} to be {amountWhitelisted} but found {found}");
            contributed.Should().Be(0, $"Expected {address} not have any contributions place as of yet!");
        }

        [Fact]
        public async Task CannotBuyMoreThanAllowed()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            Initialize(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            });
            string address = AccountDictionary.Last().Key;
            var amountWhitelisted = GetEther(1);
            var amountContributed = GetEther(5);

            //Wait for the tokensale to start
            while (DateTime.UtcNow < openingtime)
                Thread.Sleep(TimeSpan.FromSeconds(1));

            var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, address, amountWhitelisted);

            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), address);

            //Act
            Func<Task> exceptionAction = async () => await BuyTokens(address, address, amountContributed, contractLinkBuyer);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
            var found = await GetCurrentWhitelistInfo(address);
            var contributed = await GetCurrentWhitelistContribution(address);
            found.Should().BeGreaterThan(0, $"Expected {address} to be whitelisted!");
            found.Should().Be(amountWhitelisted,
                $"Expected whitelisted amount for address {address} to be {amountWhitelisted} but found {found}");
            contributed.Should().Be(0, $"Expected {address} not have any contributions place as of yet!");
        }

        [Fact]
        public async Task CorrectAmountContributedForWhitelist()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            Initialize(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            });
            string address = AccountDictionary.Last().Key;
            var amountWhitelisted = GetEther(5);
            var amountContributed = GetEther(1);

            //Wait for the tokensale to start
            while (DateTime.UtcNow < openingtime)
                Thread.Sleep(TimeSpan.FromSeconds(1));

            var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, address, amountWhitelisted);

            var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), address);

            //Act
            var tokenbuyTransactionRecepit = await BuyTokens(address, address, amountContributed, contractLinkBuyer);

            //Assert
            var found = await GetCurrentWhitelistInfo(address);
            var contributed = await GetCurrentWhitelistContribution(address);
            found.Should().BeGreaterThan(0, $"Expected {address} to be whitelisted!");
            found.Should().Be(amountWhitelisted,
                $"Expected whitelisted amount for address {address} to be {amountWhitelisted} but found {found}");
            contributed.Should().Be(amountContributed, $"Expected {address} not have any contributions place as of yet!");
        }

        [Fact]
        public async Task CorrectAmountRegisteredForWhitelist()
        {
            //Arrange
            Initialize();
            string address = await CreateNewEthereumAddressAsync(Password);
            int amount = 20000;

            //Act
            var result = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, address, amount);

            //Assert
            var found = await GetCurrentWhitelistInfo(address);
            var contributed = await GetCurrentWhitelistContribution(address);
            found.Should().BeGreaterThan(0, $"Expected {address} to be whitelisted!");
            found.Should().Be(amount,
                $"Expected whitelisted amount for address {address} to be {amount} but found {found}");
            contributed.Should().Be(0, $"Expected {address} not have any contributions place as of yet!");
        }

        [Fact]
        public async Task OwnerCanAddToWhitelistTest()
        {
            //Arrange
            Initialize();
            string address = await CreateNewEthereumAddressAsync(Password);
            int amount = 100;

            //Act
            var result = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, address, amount);

            //Assert
            result.Status.HexValue.Should().Be("0x01");
            var found = await GetCurrentWhitelistInfo(address);
            found.Should().Be(amount, $"Expected {address} to be whitelisted!");
        }

        [Fact]
        public async Task OwnerCanChangeWhitelistTest()
        {
            //Arrange
            Initialize();
            string address = await CreateNewEthereumAddressAsync(Password);
            int amount = 100;

            //Act
            var result = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, address, amount);

            amount = amount * 2;
            result = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                AccountDictionary.ElementAt(0).Key, address, amount);

            //Assert
            result.Status.HexValue.Should().Be("0x01");
            var found = await GetCurrentWhitelistInfo(address);
            found.Should().Be(amount, $"Expected {address} to be whitelisted!");
        }

        [Fact]
        public async Task StrangerCannotAddToWhitelistTest()
        {
            //Arrange
            Initialize();
            var fromAddress = AccountDictionary.Last().Key;
            var contract = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), fromAddress);

            //Act
            Func<Task> exceptionAction = async () => await ExecuteFunc(contract.GetFunction("setUserCap"),
               fromAddress, fromAddress, 50000);

            //Assert
            exceptionAction.Should().Throw<RpcResponseException>().And.Message.Should().Contain("revert");
            var found = await GetCurrentWhitelistInfo(fromAddress);
            found.Should().Be(0, $"Expected {fromAddress} not to be whitelisted!");
        }

        [Fact]
        public async Task UnknownWhitelistAddressHasNoAmount()
        {
            //Arrange
            Initialize();
            string address = await CreateNewEthereumAddressAsync(Password);

            //Act

            //Assert
            var found = await GetCurrentWhitelistInfo(address);
            found.Should().Be(0, $"Expected {address} not to be whitelisted!");
        }

        [Fact]
        public async Task UnknownWhitelistAddressHasNoContribution()
        {
            //Arrange
            Initialize();
            string address = await CreateNewEthereumAddressAsync(Password);

            //Act

            //Assert
            var found = await GetCurrentWhitelistContribution(address);
            found.Should().Be(0, $"Expected {address} not to have any contributions!");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns the addresses whitelist contribution.
        /// </summary>
        /// <returns></returns>
        private async Task<BigInteger> GetCurrentWhitelistContribution(string address) =>
            await CrowdSaleContract.GetFunction("getUserContribution").CallAsync<BigInteger>(address);

        /// <summary>
        /// Returns the addresses currently on the whitelist.
        /// </summary>
        /// <returns></returns>
        private async Task<BigInteger> GetCurrentWhitelistInfo(string address) =>
            await CrowdSaleContract.GetFunction("getUserCap").CallAsync<BigInteger>(address);

        #endregion Private Methods
    }
}