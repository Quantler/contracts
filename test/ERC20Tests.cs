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