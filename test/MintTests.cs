using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    public class MintTests:BaseTest
    {
        /// <summary>
        /// Initialize MintTests
        /// </summary>
        /// <param name="output"></param>
        public MintTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task OwnerCanMintTokensDuringSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task OwnerCannotMintTokensAfterSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task NonOwnerCannotMintTokensDuringSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task NonOwnerCannotMintTokensAfterSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

    }
}
