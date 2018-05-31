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
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace test.Model
{
    public class CrowdSaleBuilder
    {
        #region Public Properties

        public Dictionary<string, BigInteger> Contributions { get; private set; }
        public Action<CrowdsaleConstructorModel> InitializeAction { get; private set; }

        public Func<CrowdsaleConstructorModel, Task> PrepareAction { get; private set; }
        public Func<CrowdsaleConstructorModel, bool> WaitAfter { get; private set; }
        public Func<CrowdsaleConstructorModel, bool> WaitBefore { get; private set; }

        #endregion Public Properties

        #region Public Methods

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

        public CrowdSaleBuilder WaitAfterContributing(Func<CrowdsaleConstructorModel, bool> waitafter)
        {
            WaitAfter = waitafter;
            return this;
        }

        public CrowdSaleBuilder WaitBeforeContributing(Func<CrowdsaleConstructorModel, bool> waitBefore)
        {
            WaitBefore = waitBefore;
            return this;
        }

        public CrowdSaleBuilder WithContributors(Dictionary<string, BigInteger> contributors)
        {
            Contributions = contributors;
            return this;
        }

        #endregion Public Methods
    }
}