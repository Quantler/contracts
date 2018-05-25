using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public class CrowdSaleBuilder
    {
        public Action<CrowdsaleConstructorModel> InitializeAction { get; private set; }

        public Func<CrowdsaleConstructorModel, Task> PrepareAction { get; private set; }

        public Dictionary<string, BigInteger> Contributions { get; private set; }

        public Func<CrowdsaleConstructorModel, bool> WaitBefore { get; private set; }

        public Func<CrowdsaleConstructorModel, bool> WaitAfter { get; private set; }

        public static CrowdSaleBuilder Init(Action<CrowdsaleConstructorModel> initializeAction)
        {
            var toreturn = new CrowdSaleBuilder();
            toreturn.InitializeAction = initializeAction;
            return toreturn;
        }

        public CrowdSaleBuilder Prepare(Func<CrowdsaleConstructorModel, Task> prepareAction)
        {
            PrepareAction = prepareAction;
            return this;
        }

        public CrowdSaleBuilder WithContributors(Dictionary<string, BigInteger> contributors)
        {
            Contributions = contributors;
            return this;
        }

        public CrowdSaleBuilder WaitBeforeContributing(Func<CrowdsaleConstructorModel, bool> waitBefore)
        {
            WaitBefore = waitBefore;
            return this;
        }

        public CrowdSaleBuilder WaitAfterContributing(Func<CrowdsaleConstructorModel, bool> waitafter)
        {
            WaitAfter = waitafter;
            return this;
        }
    } 
}
