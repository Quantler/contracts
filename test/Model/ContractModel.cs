using Newtonsoft.Json.Linq;

namespace test.Model
{
    public class ContractModel
    {
        #region Public Properties

        public string Abi { get; set; }
        public string ByteCode { get; set; }

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
                Abi = data["abi"].ToString()
            };
        }

        #endregion Public Methods
    }
}