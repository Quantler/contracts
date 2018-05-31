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
using Xunit;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Applicable Tests:
    ///     1. CanDeployCorrectContract => base test for deploying the tokensale contract and the crowdsale contract
    /// </summary>
    public class DeployTests : BaseTest
    {
        #region Public Constructors

        /// <summary>
        /// Initialize DeployTests
        /// </summary>
        /// <param name="output"></param>
        public DeployTests(ITestOutputHelper output) : base(output) { }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public void CanDeployCorrectContract()
        {
            //Arrange

            //Act
            Initialize();

            //Assert
            TokenContract.Should().NotBeNull();
            TokenContract.Address.Should().NotBeNullOrWhiteSpace();
            CrowdSaleContract.Should().NotBeNull();
            CrowdSaleContract.Address.Should().NotBeNullOrWhiteSpace();
        }

        #endregion Public Methods
    }
}