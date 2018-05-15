using FluentAssertions;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. GetCorrectInitialSupplyTest => get correct initial supply, which is 0
    ///     2. GetCorrectTokenNameTest => get correct token name, which is Quantler
    ///     3. GetCorrectTokenSymbolTest => get correct token symbol, which is QUANT
    /// </summary>
    /// <seealso cref="test.BaseTest" />
    public class ERC20Tests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize ERC20Tests
        /// </summary>
        /// <param name="output"></param>
        public ERC20Tests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task GetCorrectInitialSupplyTest()
        {
            //Arrange
            Initialize();
            var func = TokenContract.GetFunction("totalSupply");

            //Act
            var result = await func.CallAsync<BigInteger>();

            //Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetCorrectTokenNameTest()
        {
            //Arrange
            Initialize();
            var func = TokenContract.GetFunction("name");

            //Act
            var result = await func.CallAsync<string>();

            //Assert
            result.Should().Be("Quantler");
        }

        [Fact]
        public async Task GetCorrectTokenSymbolTest()
        {
            //Arrange
            Initialize();
            var func = TokenContract.GetFunction("symbol");

            //Act
            var result = await func.CallAsync<string>();

            //Assert
            result.Should().Be("QUANT");
        }

        #endregion Public Methods
    }
}