using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    public class TransferTests : BaseTest
    {
        /// <summary>
        /// Initialize TransferTests
        /// </summary>
        /// <param name="output"></param>
        public TransferTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task NoTokensInWalletAfterPurchaseTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotWithdrawTokensFromContractNonOwnerTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotWithdrawTokensFromContractDuringSaleTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CanWithdrawTokensFromContractAfterSaleOwnerTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CannotWithdrawTokensFromContractAfterSaleNonOwnerTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task CanTransferTokensFromWalletToOtherWalletAfterRelase()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }
    }
}
