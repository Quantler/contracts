using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. NoTokensInWalletAfterPurchaseTest => tokens purchased should not be present in the wallet of the purchaser as of yet
    ///     2. CannotWithdrawTokensFromContractDuringSaleTest => during the crowdsale, owner cannot withdraw tokens
    ///     3. CanWithdrawTokensFromContractAfterSaleOwnerTest => owner can withdraw tokens from contract after sale has been concluded
    ///     4. CanTransferTokensFromWalletToOtherWalletAfterRelase =>
    ///     5. CanWithdrawTokensFromContractToAllAfterSaleOwnerTest => owner can send a withdrawal request for all addresses after token sale has been concluded
    ///     6. CannotWithdrawTokensFromContractToAllAfterSaleOwnerTwiceTest => sending a withdrawal request twice should be useless
    ///     7. CannotWithdrawTokensFromContractToAllAfterSaleNonOwnerTest => non-owners cannot send a withdrawal request for all addresses after token sale has been conculded
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
        public async Task CannotWithdrawTokensFromContractDuringSaleTest()
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

        [Fact]
        public async Task CanWithdrawTokensFromContractAfterSaleOwnerTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        public async Task NoTokensInWalletAfterPurchaseTest()
        {
            throw new NotImplementedException();
            //Arrange

            //Act

            //Assert
        }

        #endregion Public Methods
    }
}