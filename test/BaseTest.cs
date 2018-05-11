using Microsoft.Extensions.Configuration;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace test
{
    /// <summary>
    /// Base test file. Every test derives from this file for easier testing.
    /// </summary>
    public abstract class BaseTest
    {
        #region Protected Fields

        /// <summary>
        /// One ETH in Wei
        /// </summary>
        protected const long OneEth = 100000000000000000;

        /// <summary>
        /// The configuration
        /// </summary>
        protected static IConfigurationRoot Configuration;

        /// <summary>
        /// The web3
        /// </summary>
        protected Web3 Web3;

        #endregion Protected Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTest"/> class.
        /// </summary>
        protected BaseTest()
        {
            if (Configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                Configuration = builder.Build();

                //Print current config
                WriteLine($"========== BASE CONFIGURATION =========");
                WriteLine($":");
                WriteLine($":");
                WriteLine($":");
                WriteLine($":");
                WriteLine($":");
                WriteLine($"======= END BASE CONFIGURATION =======");

                //Set references
                Web3 = new Web3(Configuration["RPC"]);
                AddressDictionary = Configuration.GetSection("TestAddress").GetChildren().ToDictionary(x => x.Key, x => x.Value);
            }
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Gets the address dictionary.
        /// </summary>
        /// <value>
        /// The address dictionary.
        /// </value>
        public Dictionary<string, string> AddressDictionary { get; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Converts from unix timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        protected DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts to unix timestamp.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        protected double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// Deploys the contract.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="contractname">The contractname.</param>
        /// <param name="parms">The parms.</param>
        /// <returns>The transaction receipt</returns>
        protected TransactionReceipt DeployContract(ContractModel contract, string owner, string contractname, params object[] parms)
        {
            //Deploy
            string tx = Web3.Eth.DeployContract.SendRequestAsync("", contract.ByteCode, owner, GetEnoughGas(), parms)
                .Result;
            return GetReceiptFromTransaction(tx);
        }

        /// <summary>
        /// Gets the contract from the blockchain.
        /// </summary>
        /// <param name="contractModel">The contract model.</param>
        /// <param name="contractAddress">The contract address.</param>
        /// <returns></returns>
        protected Contract GetContract(ContractModel contractModel, string contractAddress) =>
            Web3.Eth.GetContract("", contractAddress);

        /// <summary>
        /// Gets the contract model.
        /// </summary>
        /// <param name="contractname">The contractname.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected ContractModel GetContractModel(string contractname)
        {
            //Get the source file
            FileInfo sourceFile = new FileInfo(Path.Combine("..", "build", "contracts", $"{contractname}.json"));
            if (!sourceFile.Exists)
                throw new Exception($"Built contract of name {contractname} does not exist. Expected path: {sourceFile.FullName}");

            //Parse content to contract model
            return ContractModel.FromJson(File.ReadAllText(sourceFile.FullName));
        }

        /// <summary>
        /// Gets enough gas.
        /// </summary>
        /// <returns></returns>
        protected HexBigInteger GetEnoughGas() => new HexBigInteger(2000000);

        /// <summary>
        /// Monitors the tx.
        /// </summary>
        /// <param name="transactionHash">The transaction hash.</param>
        /// <returns></returns>
        protected TransactionReceipt GetReceiptFromTransaction(string transactionHash)
        {
            //Check receipt
            var receipt = Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;

            //Weit for the receipt
            while (receipt == null)
            {
                WriteLine("Sleeping for 5 seconds");
                System.Threading.Thread.Sleep(5000);
                receipt = Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;
            }

            //Report + return
            WriteLine("Contract address {0} block height {1}", receipt.ContractAddress, receipt.BlockNumber.Value);
            return receipt;
        }

        /// <summary>
        /// Initializes the token contract.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns></returns>
        protected TransactionReceipt InitializeTokenContract(string owner, string contractName) =>
            DeployContract(GetContractModel("QuantToken"), owner, contractName);

        /// <summary>
        /// Initializes the crowdsale contract.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns></returns>
        protected TransactionReceipt InitializeCrowdSaleContract(string owner, string contractName, long _openingTime, long _closingTime, 
            long _softcaprate, long _hardcaprate, long _presalerate, string _wallet, string _companyreserve, string _miningpool, string _icobounty, 
            string _githubbounty, long _cap, long _softcaperc, long _presalecap, string _token)
        {
            //Set constructor input
            object[] constructorParms = {
                _openingTime,
                _closingTime,
                _softcaprate,
                _hardcaprate,
                _presalerate,
                _wallet,
                _companyreserve,
                _miningpool,
                _icobounty,
                _githubbounty,
                _cap,
                _softcaperc,
                _presalecap,
                _token
            };

            //Return deployed contract
            return DeployContract(GetContractModel("QuantTokenCrowdSale"), owner, contractName, constructorParms);
        }

        #endregion Protected Methods
    }
}