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
using System.Threading.Tasks;
using test.Model;
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
    ///     10. CorrectAmountContributedForWhitelist => When buying tokens, the correct amount of contribution is set in state
    ///     11. OwnerCanChangeWhitelistTest => owner can change the amount a source address is allowed to spent during the crowdsale
    ///     12. CannotBuyMoreThanAllowedPreSale => buyer should not be able to buy more tokens during pre-sale than allowed
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
            string address = AccountDictionary.Last().Key;
            var amountWhitelisted = GetEther(0);
            var amountContributed = GetEther(5);

            //Prepare crowdsale
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            }).WaitBeforeContributing(x => DateTime.UtcNow < openingtime));

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
            string address = AccountDictionary.Last().Key;
            var amountWhitelisted = GetEther(1);
            var amountContributed = GetEther(5);

            //Prepare crowdsale
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            }).WaitBeforeContributing(x => DateTime.UtcNow < openingtime));

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
        public async Task CannotBuyMoreThanAllowedPreSale()
        {
            //Arrange
            var openingtime = DateTime.UtcNow.Add(OpeningTimeDelay);
            string address = AccountDictionary.Last().Key;
            var amountWhitelisted = GetEther(1);
            var amountContributed = GetEther(5);

            //Prepare crowdsale
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            }).Prepare(async x =>
            {
                //Open pre-sale
                await OpenPreSale(AccountDictionary.ElementAt(0).Key);

                //Whitelist person
                var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                    AccountDictionary.ElementAt(0).Key, address, amountWhitelisted);
            }));

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
            string address = AccountDictionary.Last().Key;
            var amountWhitelisted = GetEther(5);
            var amountContributed = GetEther(1);

            //Prepare crowdsale
            await PrepareCrowdSale(CrowdSaleBuilder.Init(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(openingtime);
            }).WaitBeforeContributing(x => DateTime.UtcNow < openingtime));

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