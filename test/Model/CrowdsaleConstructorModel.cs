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

using System;
using System.Linq;
using System.Numerics;

namespace test.Model
{
    public class CrowdsaleConstructorModel
    {
        #region Public Properties

        public long ClosingTime { get; set; } = BaseTest.ConvertToUnixTimestamp(DateTime.UtcNow.AddDays(62));
        public string CompanyReserve { get; set; } = BaseTest.AccountDictionary.ElementAt(2).Key;
        public string GitHubBounty { get; set; } = BaseTest.AccountDictionary.ElementAt(5).Key;
        public BigInteger HardCap { get; set; } = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(35000));
        public BigInteger HardCapRate { get; set; } = 1234;
        public string ICOBounty { get; set; } = BaseTest.AccountDictionary.ElementAt(4).Key;
        public string MiningPool { get; set; } = BaseTest.AccountDictionary.ElementAt(3).Key;
        public long OpeningTime { get; set; } = BaseTest.ConvertToUnixTimestamp(DateTime.UtcNow.AddDays(31));
        public string Owner { get; set; } = BaseTest.AccountDictionary.ElementAt(0).Key;
        public BigInteger PreSaleCap { get; set; } = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(1000));
        public BigInteger PreSaleRate { get; set; } = 1666;
        public BigInteger SofCap { get; set; } = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(12500));
        public BigInteger SoftCapRate { get; set; } = 1481;
        public string Wallet { get; set; } = BaseTest.AccountDictionary.ElementAt(1).Key;

        #endregion Public Properties
    }
}