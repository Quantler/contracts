using System;
using System.Linq;
using System.Numerics;

namespace test.Model
{
    public class CrowdsaleConstructorModel
    {
        public string Owner { get; set; } = BaseTest.AccountDictionary.ElementAt(0).Key;
        public string CompanyReserve { get; set; } = BaseTest.AccountDictionary.ElementAt(2).Key;
        public string Wallet { get; set; } = BaseTest.AccountDictionary.ElementAt(1).Key;
        public string MiningPool { get; set; } = BaseTest.AccountDictionary.ElementAt(3).Key;
        public string ICOBounty { get; set; } = BaseTest.AccountDictionary.ElementAt(4).Key;
        public string GitHubBounty { get; set; } = BaseTest.AccountDictionary.ElementAt(5).Key;
        public long OpeningTime { get; set; } = BaseTest.ConvertToUnixTimestamp(DateTime.UtcNow.AddDays(31));
        public long ClosingTime { get; set; } = BaseTest.ConvertToUnixTimestamp(DateTime.UtcNow.AddDays(62));
        public long SoftCapRate { get; set; } = 1542;
        public long HardCapRate { get; set; } = 1234;
        public long PreSaleRate { get; set; } = 1666;
        public BigInteger HardCap { get; set; } = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(35000));
        public BigInteger SofCap { get; set; }  = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(12500));
        public BigInteger PreSaleCap { get; set; } = BigInteger.Multiply(BaseTest.OneEth, new BigInteger(1000));
    }
}