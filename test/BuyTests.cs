using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using test.Model;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    public class BuyTests : BaseTest
    {
        /// <summary>
        /// Initialize BuyTests
        /// </summary>
        /// <param name="output"></param>
        public BuyTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task CannotBuyTokensIfTokenSaleNotStarted()
        {
            //Arrange
            Initialize();
            var fromAddress = await CreateNewEthereumAddressAsync(Password);

            //Act
            var result = await BuyTokens(fromAddress, fromAddress, GetEther(1));

            //Assert
            result.Status.Should().Be("");
            (await GetBalance(fromAddress)).Should().Be(0);
        }

        [Fact]
        public async Task CanBuyTokensIfPreSaleTokenSaleStarted()
        {
            //Arrange
            Initialize();
            var fromAddress = await CreateNewEthereumAddressAsync(Password);

            //Start presale
            var preSaleStart = await CrowdSaleContract.GetFunction("openPresale")
                .SendTransactionAndWaitForReceiptAsync(AccountDictionary.ElementAt(0).Key);

            //Act
            var result = await BuyTokens(fromAddress, fromAddress, GetEther(1));

            //Assert
            result.Status.Should().Be("");
            (await GetBalance(fromAddress)).Should().Be(0);
        }

        [Fact]
        public async Task CanBuyTokensIfSoftCapSaleTokenSaleStarted()
        {
            //Arrange
            Initialize(x => x.OpeningTime = ConvertToUnixTimestamp(DateTime.UtcNow.AddHours(-1)));
            var fromAddress = await CreateNewEthereumAddressAsync(Password);
            long expectedAmount = 1 * (new CrowdsaleConstructorModel().SoftCapRate);
            var addWhitelist = await CrowdSaleContract.GetFunction("setUserCap").CallAsync<string>(fromAddress, GetEther(1));

            //Act
            var result = await BuyTokens(fromAddress, fromAddress, GetEther(1));

            //Assert
            addWhitelist.Should().NotBeNullOrWhiteSpace();
            result.Status.Should().Be("");
            (await GetBalance(fromAddress)).Should().Be(expectedAmount);
        }

        [Fact]
        public async Task CanBuyTokensIfHardCapSaleTokenSaleStarted()
        {
            //Arrange
            Initialize(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(DateTime.UtcNow.AddHours(-1));
                x.SofCap = 0;
            });
            var fromAddress = await CreateNewEthereumAddressAsync(Password);
            long expectedAmount = 1 * (new CrowdsaleConstructorModel().HardCapRate);
            var addWhitelist = await CrowdSaleContract.GetFunction("setUserCap").CallAsync<string>(fromAddress, GetEther(1));

            //Act
            var result = await BuyTokens(fromAddress, fromAddress, GetEther(1));

            //Assert
            addWhitelist.Should().NotBeNullOrWhiteSpace();
            result.Status.Should().Be("");
            (await GetBalance(fromAddress)).Should().Be(expectedAmount);
        }

        [Fact]
        public async Task CannotBuyMoreTokensThanSupply()
        {
            //Arrange
            Initialize(x =>
            {
                x.OpeningTime = ConvertToUnixTimestamp(DateTime.UtcNow.AddHours(-1));
                x.SofCap = 0;
                x.PreSaleCap = 0;
                x.HardCap = 1;
            });
            var fromAddress = await CreateNewEthereumAddressAsync(Password);
            long expectedAmount = 2 * (new CrowdsaleConstructorModel().HardCapRate);
            var addWhitelist = await CrowdSaleContract.GetFunction("setUserCap").CallAsync<string>(fromAddress, GetEther(2));

            //Act
            var result = await BuyTokens(fromAddress, fromAddress, GetEther(1));

            //Assert
            addWhitelist.Should().NotBeNullOrWhiteSpace();
            result.Status.Should().Be("");
            (await GetBalance(fromAddress)).Should().Be(expectedAmount);
        }

        [Fact]
        public async Task CannotBuyNegativeAmountOfTokens()
        {
            //Arrange
            Initialize(x => x.OpeningTime = ConvertToUnixTimestamp(DateTime.UtcNow.AddHours(-1)));
            var fromAddress = await CreateNewEthereumAddressAsync(Password);
            var addWhitelist = await CrowdSaleContract.GetFunction("setUserCap").CallAsync<string>(fromAddress, GetEther(1));

            //Act
            var result = await BuyTokens(fromAddress, fromAddress, GetEther(-1));

            //Assert
            addWhitelist.Should().NotBeNullOrWhiteSpace();
            result.Status.Should().Be("");
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
        public async Task CorrectOverflowFromSoftCapToHardCapRateTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CorrectOverflowFromSoftCapToHardCapTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotBuyMoreThanHardCapTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CorrectWalletSpreadPreSale()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CorrectWalletSpreadSoftCapSale()
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
    }
}
