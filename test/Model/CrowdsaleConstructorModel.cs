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
        public BigInteger HardCapRate { get; set; } = 675154320987654;
        public string ICOBounty { get; set; } = BaseTest.AccountDictionary.ElementAt(4).Key;
        public string MiningPool { get; set; } = BaseTest.AccountDictionary.ElementAt(3).Key;
        public long OpeningTime { get; set; } = BaseTest.ConvertToUnixTimestamp(DateTime.UtcNow.AddDays(31));
        public string Owner { get; set; } = BaseTest.AccountDictionary.ElementAt(0).Key;
        public BigInteger PreSaleCap { get; set; } = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(1000));
        public BigInteger PreSaleRate { get; set; } = 810185185185185;
        public BigInteger SofCap { get; set; } = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(12500));
        public BigInteger SoftCapRate { get; set; } = 600137174211248;
        public string Wallet { get; set; } = BaseTest.AccountDictionary.ElementAt(1).Key;

        #endregion Public Properties
    }
}