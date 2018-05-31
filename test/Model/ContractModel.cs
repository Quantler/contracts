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

using Newtonsoft.Json.Linq;

namespace test.Model
{
    public class ContractModel
    {
        #region Public Properties

        /// <summary>
        /// Gets the abi definition.
        /// </summary>
        public string Abi { get; private set; }

        /// <summary>
        /// Gets the byte code of the contract.
        /// </summary>
        public string ByteCode { get; private set; }

        /// <summary>
        /// Gets the contract name.
        /// </summary>
        public string Name { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the contract model from a json file
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static ContractModel FromJson(string json)
        {
            //Get data
            var data = JObject.Parse(json);
            return new ContractModel
            {
                ByteCode = data["bytecode"].Value<string>(),
                Abi = data["abi"].ToString(),
                Name = data["contractName"].Value<string>()
            };
        }

        #endregion Public Methods
    }
}