﻿#region License

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

using Microsoft.Extensions.Configuration;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using test.Model;
using Xunit.Abstractions;

namespace test
{
    /// <summary>
    /// Base test file. Every test derives from this file for easier testing.
    /// </summary>
    public abstract class BaseTest
    {
        #region Public Fields

        /// <summary>
        /// One ETH in Wei
        /// </summary>
        public static BigInteger OneEth = new BigInteger(100000000000000000);

        #endregion Public Fields

        #region Protected Fields

        /// <summary>
        /// The crowd sale contract name
        /// </summary>
        protected const string CrowdSaleContractName = "QuantTokenCrowdSale";

        /// <summary>
        /// The password
        /// </summary>
        protected const string Password = "Password";

        /// <summary>
        /// The token contract name
        /// </summary>
        protected const string TokenContractName = "QuantToken";

        /// <summary>
        /// The configuration
        /// </summary>
        protected static IConfigurationRoot Configuration;

        /// <summary>
        /// Outputhelper instance
        /// </summary>
        protected readonly ITestOutputHelper Output;

        /// <summary>
        /// The web3
        /// </summary>
        protected Web3 OwnerWeb3;

        #endregion Protected Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTest"/> class.
        /// </summary>
        protected BaseTest(ITestOutputHelper output)
        {
            Output = output;
            if (Configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                Configuration = builder.Build();

                //Print current config
                AccountDictionary = Configuration.GetSection("Address").GetChildren().ToArray()
                    .ToDictionary(x => $"{Configuration[x.Path + ":0"]}", x => $"{Configuration[x.Path + ":1"]}");
                //output.WriteLine("========== BASE CONFIGURATION =========");
                //output.WriteLine($"TestAddress:{AccountDictionary.Select(x => $"\n\r{x.Key}: {x.Value}")}");
                //output.WriteLine($"RPC:{Configuration["RPC"]}");
                //output.WriteLine("======= END BASE CONFIGURATION =======");
            }

            //Set references
            OwnerWeb3 = GetClientConnection(AccountDictionary.ElementAt(0).Value);
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Gets the address dictionary.
        /// </summary>
        /// <value>
        /// The address dictionary.
        /// </value>
        public static Dictionary<string, string> AccountDictionary { get; private set; }

        /// <summary>
        /// Gets the crowdsale constructor model.
        /// </summary>
        public CrowdsaleConstructorModel CrowdsaleConstructorModel { get; private set; }

        /// <summary>
        /// Gets or sets the crowd sale contract.
        /// </summary>
        public Contract CrowdSaleContract { get; set; }

        /// <summary>
        /// Gets the crowd sale receipt.
        /// </summary>
        public TransactionReceipt CrowdSaleReceipt { get; private set; }

        /// <summary>
        /// Gets the token contract.
        /// </summary>
        public Contract TokenContract { get; private set; }

        /// <summary>
        /// Gets the token receipt.
        /// </summary>
        public TransactionReceipt TokenReceipt { get; private set; }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Gets the opening time delay for the main sale.
        /// </summary>
        protected TimeSpan OpeningTimeDelay => TimeSpan.FromSeconds(2);

        #endregion Protected Properties

        #region Public Methods

        /// <summary>
        /// Converts from unix timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts to unix timestamp.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Convert.ToInt64(Math.Floor(diff.TotalSeconds));
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Execute the buy tokens function based on the supplied info
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="crowdsaleContract"></param>
        /// <returns></returns>
        protected async Task<TransactionReceipt> BuyTokens(string fromAddress, string toAddress, BigInteger amount, Contract crowdsaleContract = null)
        {
            //Get send function
            var func = (crowdsaleContract ?? CrowdSaleContract).GetFunction("buyTokens");

            //Send transaction and return results
            var result = await func.SendTransactionAndWaitForReceiptAsync(fromAddress, GetEnoughGas(), new HexBigInteger(amount), null, toAddress);
            Output.WriteLine($"Buying tokens GasUsed: {result.GasUsed.Value}, CumulativeGasUsed: {result.CumulativeGasUsed.Value}");
            return result;
        }

        /// <summary>
        /// Creates the new ethereum address asynchronous.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected async Task<string> CreateNewEthereumAddressAsync(string password)
        {
            var address = await OwnerWeb3.Personal.NewAccount.SendRequestAsync(password);
            Output.WriteLine($"Generated Address: {address}");
            return address;
        }

        /// <summary>
        /// Deploys the contract.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="parms">The parms.</param>
        /// <returns>The transaction receipt</returns>
        protected Contract DeployContract(ContractModel contract, string owner, out TransactionReceipt receipt, params object[] parms)
        {
            //Deploy
            receipt = OwnerWeb3.Eth.DeployContract
                .SendRequestAndWaitForReceiptAsync(contract.Abi, contract.ByteCode, owner, GetEnoughGas(), null, parms)
                .Result;
            Output.WriteLine($"Contract ({contract.Name}) address {receipt.ContractAddress} block height {receipt.BlockNumber.Value}");
            return OwnerWeb3.Eth.GetContract(contract.Abi, receipt.ContractAddress);
        }

        /// <summary>
        /// Executes the function specified.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <param name="from">From.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        protected async Task<TransactionReceipt> ExecuteFunc(Function func, string from, params object[] input)
        {
            //Estimate gas
            var gas = await func.EstimateGasAsync(from, null, null, input);
            Output.WriteLine($"Burning gas: {gas.Value} for contract: {func.ContractAddress}");

            //Send transaction and return results
            return await func.SendTransactionAndWaitForReceiptAsync(from, GetEnoughGas(), null, null, input);
        }

        /// <summary>
        /// Gets the account with password.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        protected ManagedAccount GetAccount(string address = "")
        {
            //Check input
            if (!AccountDictionary.ContainsKey(address))
                throw new Exception($"Expected address to be known {address}");
            address = string.IsNullOrWhiteSpace(address) ? AccountDictionary.ElementAt(0).Key : address;

            //Return account
            return new ManagedAccount(address, AccountDictionary[address]);
        }

        /// <summary>
        /// Returns the allocated balance for the selected address
        /// </summary>
        /// <returns></returns>
        protected async Task<BigInteger> GetAllocatedBalance(string address) =>
            await CrowdSaleContract.GetFunction("balances").CallAsync<BigInteger>(address);

        /// <summary>
        /// Get balance of address based on contract reference
        /// </summary>
        protected async Task<BigInteger> GetBalance(string address, Contract tokenContract = null) =>
            await (tokenContract ?? TokenContract).GetFunction("balanceOf")
                .CallAsync<BigInteger>(address);

        /// <summary>
        /// Gets the client connection, authenticated
        /// </summary>
        /// <param name="_privateKey"></param>
        /// <returns></returns>
        protected Web3 GetClientConnection(string _privateKey) =>
            new Web3(new Account(_privateKey), Configuration["RPC"]);

        /// <summary>
        /// Gets the client connection, authenticated
        /// </summary>
        /// <param name="_account"></param>
        /// <returns></returns>
        protected Web3 GetClientConnection(ManagedAccount _account) =>
            new Web3(_account, Configuration["RPC"]);

        /// <summary>
        /// Gets the contract from the specified address.
        /// </summary>
        /// <param name="contractAddress">The contract address.</param>
        /// <param name="contractModel">The contract model.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected async Task<Contract> GetContractFromAddress(string contractAddress, ContractModel contractModel, string fromAddress = null, string password = "")
        {
            //Get account, if applicable
            password = AccountDictionary.ContainsKey(fromAddress) ? AccountDictionary[fromAddress] : string.IsNullOrWhiteSpace(password) ? Password : password;
            var account = new ManagedAccount(string.IsNullOrWhiteSpace(fromAddress) ? await CreateNewEthereumAddressAsync(password) : fromAddress, password);
            Web3 web3Client = new Web3(account, Configuration["RPC"]);

            //Return contract address using account details
            return web3Client.Eth.GetContract(contractModel.Abi, contractAddress);
        }

        /// <summary>
        /// Gets the contract model.
        /// </summary>
        /// <param name="contractname">The contractname.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected ContractModel GetContractModel(string contractname)
        {
            //Get the source file
            FileInfo sourceFile = new FileInfo(Path.GetFullPath(Path.Combine("..", "..", "..", "..", "build", "contracts", $"{contractname}.json")));
            if (!sourceFile.Exists)
                throw new Exception($"Built contract of name {contractname} does not exist. Expected path: {sourceFile.FullName}");

            //Parse content to contract model
            return ContractModel.FromJson(File.ReadAllText(sourceFile.FullName));
        }

        /// <summary>
        /// Gets enough gas.
        /// </summary>
        /// <returns></returns>
        protected HexBigInteger GetEnoughGas(long _gasAmount = 6721975) => new HexBigInteger(_gasAmount);

        /// <summary>
        /// Gets some ether amount.
        /// </summary>
        /// <param name="_ether">The ether.</param>
        /// <returns></returns>
        protected BigInteger GetEther(double _ether) =>
            UnitConversion.Convert.ToWei(_ether);

        /// <summary>
        /// Gets the fractional amount of tokens.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        protected double GetFractionalAmountOfTokens(BigInteger amount) => (double)amount / Math.Pow(10, 18);

        /// <summary>
        /// Monitors the tx.
        /// </summary>
        /// <param name="transactionHash">The transaction hash.</param>
        /// <returns></returns>
        protected TransactionReceipt GetReceiptFromTransaction(string transactionHash)
        {
            //Check receipt
            var receipt = OwnerWeb3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;

            //Weit for the receipt
            while (receipt == null)
            {
                Output.WriteLine("Sleeping for 5 seconds");
                System.Threading.Thread.Sleep(5000);
                receipt = OwnerWeb3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;
            }

            //Report + return
            Output.WriteLine("Contract address {0} block height {1}", receipt.ContractAddress, receipt.BlockNumber.Value);
            return receipt;
        }

        /// <summary>
        /// Initializes the specified token and crowdsale contract.
        /// </summary>
        /// <param name="_constructFunc"></param>
        /// <param name="_tokenContract"></param>
        /// <param name="_crowdSaleContract"></param>
        protected void Initialize(Action<CrowdsaleConstructorModel> _constructFunc = null, Contract _tokenContract = null, Contract _crowdSaleContract = null)
        {
            //Check for input and initialize when needed
            TokenContract = _tokenContract ?? InitializeTokenContract(AccountDictionary.ElementAt(0).Key);
            CrowdSaleContract = _crowdSaleContract ?? InitializeCrowdSaleContract(TokenReceipt.ContractAddress, _constructFunc);
        }

        /// <summary>
        /// Initializes the crowdsale contract.
        /// </summary>
        /// <param name="_tokenaddress"></param>
        /// <param name="inputFunc"></param>
        /// <returns></returns>
        protected Contract InitializeCrowdSaleContract(string _tokenaddress, Action<CrowdsaleConstructorModel> inputFunc = null)
        {
            //Transform input
            var input = new CrowdsaleConstructorModel();
            inputFunc?.Invoke(input);

            //Check input
            if (string.IsNullOrWhiteSpace(_tokenaddress))
                throw new ArgumentNullException(nameof(_tokenaddress), "Token address is missing");
            else
                CrowdsaleConstructorModel = input;

            //Set constructor input
            object[] constructorParms = {
                input.OpeningTime,
                input.ClosingTime,
                input.SoftCapRate,
                input.HardCapRate,
                input.PreSaleRate,
                input.Wallet,
                input.CompanyReserve,
                input.MiningPool,
                input.ICOBounty,
                input.GitHubBounty,
                input.HardCap,
                input.SofCap,
                input.PreSaleCap,
                _tokenaddress
            };

            //Return deployed contract
            var toreturn = DeployContract(GetContractModel(CrowdSaleContractName), input.Owner, out var crowdSaleReceipt, constructorParms);
            CrowdSaleReceipt = crowdSaleReceipt;

            //Set the token minting owner to the crowdsale contract
            ExecuteFunc(TokenContract.GetFunction("transferOwnership"), input.Owner, toreturn.Address).Wait();

            //Return what we have
            return toreturn;
        }

        /// <summary>
        /// Initializes the token contract.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns></returns>
        protected Contract InitializeTokenContract(string owner)
        {
            TokenContract = DeployContract(GetContractModel(TokenContractName), owner, out var tokenReceipt);
            TokenReceipt = tokenReceipt;
            return TokenContract;
        }

        /// <summary>
        /// Opens the presale
        /// </summary>
        /// <returns></returns>
        protected async Task<TransactionReceipt> OpenPreSale(string fromAddress) =>
            await ExecuteFunc(CrowdSaleContract.GetFunction("openPresale"), fromAddress);

        /// <summary>
        /// Prepares the crowd sale.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        protected async Task PrepareCrowdSale(CrowdSaleBuilder builder)
        {
            //Arrange
            Initialize(builder.InitializeAction);

            //Open tokensale pre-sale
            if (builder.PrepareAction != null)
                await builder.PrepareAction(CrowdsaleConstructorModel);

            //Wait for the tokensale to start
            if (builder.WaitBefore != null)
                while (builder.WaitBefore(CrowdsaleConstructorModel))
                    Thread.Sleep(TimeSpan.FromSeconds(1));

            //Process sales
            if (builder.Contributions != null && builder.Contributions.Count > 0)
                foreach (var contributor in builder.Contributions)
                {
                    //Open to whitelist
                    var whitelistTransactionReceipt = await ExecuteFunc(CrowdSaleContract.GetFunction("setUserCap"),
                        AccountDictionary.ElementAt(0).Key, contributor.Key, contributor.Value);

                    //Get contract
                    var contractLinkBuyer = await GetContractFromAddress(CrowdSaleContract.Address, GetContractModel(CrowdSaleContractName), contributor.Key);

                    //Buy tokens
                    var tokenbuyTransactionRecepit = await BuyTokens(contributor.Key, contributor.Key, contributor.Value, contractLinkBuyer);
                    var resultInvestor = await GetAllocatedBalance(contributor.Key);
                }

            //Wait for the tokensale to end
            if (builder.WaitAfter != null)
                while (builder.WaitAfter(CrowdsaleConstructorModel))
                    Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        #endregion Protected Methods
    }
}