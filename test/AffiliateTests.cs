using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    public class AffiliateTests : BaseTest
    {
        /// <summary>
        /// Initialize AffiliateTests
        /// </summary>
        /// <param name="output"></param>
        public AffiliateTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task OwnerCanAddAffiliateTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task NonOwnerCannotAddAffiliateTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task NonAffiliateDoesNotGetTokens()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task AffiliateDoesGetTokens()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task AffiliateGetCorrectTokenAmountPreSale()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task AffiliateGetCorrectTokenAmountMainSoftCaoSale()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task AffiliateGetCorrectTokenAmountMainHardCapSale()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        /// <summary>
        /// Returns the addresses currently on the whitelist.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCurrentgetAffiliate(string investorAddress) =>
            await CrowdSaleContract.GetFunction("getAffiliate").CallAsync<string>(investorAddress);
    }
}
