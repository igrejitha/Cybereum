using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
//using Nethereum.BlockchainStore.AzureTables.Bootstrap;
//using Nethereum.BlockchainStore.AzureTables.Repositories;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.BlockchainProcessing.Processor;
using System.Web.Routing;
using Nethereum.RPC;

namespace Cybereum.Controllers
{
    public class NetherumController : Controller
    {

        // GET: Netherum
        public ActionResult Index()
        {
            GetBlockNumber().Wait();
            return View();
        }

        static async Task GetBlockNumber()
        {
            
            var blocks = new List<BlockWithTransactions>();
            var transactions = new List<TransactionReceiptVO>();
            var contractCreations = new List<ContractCreationVO>();
            var filterLogs = new List<FilterLogVO>();

            var web3 = new Web3(System.Web.HttpContext.Current.Request.Url.ToString());
            //var web3 = new Web3("https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c");

            //create our processor
            var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
            {
                // inject handler for each step
                steps.BlockStep.AddSynchronousProcessorHandler(b => blocks.Add(b));
                steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
                steps.ContractCreationStep.AddSynchronousProcessorHandler(cc => contractCreations.Add(cc));
                steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
            });

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
              toBlockNumber: new BigInteger(2830145),
              cancellationToken: cancellationToken,
              startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

            Console.WriteLine($"Blocks. Expected: 2, Found: {blocks.Count}");
            Console.WriteLine($"Transactions. Expected: 25, Found: {transactions.Count}");
            Console.WriteLine($"Contract Creations. Expected: 5, Found: {contractCreations.Count}");

            Log(transactions, contractCreations, filterLogs);
            //var tst = new Nethereum.JsonRpc.Client.IClient();
            //var service = new EthApiService(tst);
            //var contr = new Contract(service, "", System.Web.HttpContext.Current.Request.Url.ToString());


            //ulong totalsupply = 1000000;
            //var receipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(abi, contractByteCode, senderAddress, new Netherum.Hex.HexTypes.HexBigInteger(900000), null, totalsupply);
            //var dsfds = new Nethereum.RPC.Eth.DTOs.TransactionReceipt();

            var deploymentMessage = new StandardTokenDeployment
            {
                TotalSupply = 100000
            };
            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);
            var contractAddress = transactionReceipt.ContractAddress;
        }


        private static void Log(
    List<TransactionReceiptVO> transactions,
    List<ContractCreationVO> contractCreations,
    List<FilterLogVO> filterLogs)
        {
            Console.WriteLine("Sent From");
            foreach (var fromAddressGrouping in transactions.GroupBy(t => t.Transaction.From).OrderByDescending(g => g.Count()))
            {
                var logs = filterLogs.Where(l => fromAddressGrouping.Any((a) => l.Transaction.TransactionHash == a.TransactionHash));

                Console.WriteLine($"From: {fromAddressGrouping.Key}, Tx Count: {fromAddressGrouping.Count()}, Logs: {logs.Count()}");
            }

            Console.WriteLine("Sent To");
            foreach (var toAddress in transactions
              .Where(t => !t.Transaction.IsToAnEmptyAddress())
              .GroupBy(t => t.Transaction.To)
              .OrderByDescending(g => g.Count()))
            {
                var logs = filterLogs.Where(l => toAddress.Any((a) => l.Transaction.TransactionHash == a.TransactionHash));

                Console.WriteLine($"To: {toAddress.Key}, Tx Count: {toAddress.Count()}, Logs: {logs.Count()}");
            }

            Console.WriteLine("Contracts Created");
            foreach (var contractCreated in contractCreations)
            {
                var tx = transactions.Count(t => t.Transaction.IsTo(contractCreated.ContractAddress));
                var logs = filterLogs.Count(l => transactions.Any(t => l.Transaction.TransactionHash == t.TransactionHash));

                Console.WriteLine($"From: {contractCreated.ContractAddress}, Tx Count: {tx}, Logs: {logs}");
            }
        }

        // GET: Netherum/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Netherum/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Netherum/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Netherum/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Netherum/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Netherum/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Netherum/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }


    public class StandardTokenDeployment : ContractDeploymentMessage
    {

        public static string BYTECODE = "0x60606040526040516020806106f5833981016040528080519060200190919050505b80600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005081905550806000600050819055505b506106868061006f6000396000f360606040523615610074576000357c010000000000000000000000000000000000000000000000000000000090048063095ea7b31461008157806318160ddd146100b657806323b872dd146100d957806370a0823114610117578063a9059cbb14610143578063dd62ed3e1461017857610074565b61007f5b610002565b565b005b6100a060048080359060200190919080359060200190919050506101ad565b6040518082815260200191505060405180910390f35b6100c36004805050610674565b6040518082815260200191505060405180910390f35b6101016004808035906020019091908035906020019091908035906020019091905050610281565b6040518082815260200191505060405180910390f35b61012d600480803590602001909190505061048d565b6040518082815260200191505060405180910390f35b61016260048080359060200190919080359060200190919050506104cb565b6040518082815260200191505060405180910390f35b610197600480803590602001909190803590602001909190505061060b565b6040518082815260200191505060405180910390f35b600081600260005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008573ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925846040518082815260200191505060405180910390a36001905061027b565b92915050565b600081600160005060008673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561031b575081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505410155b80156103275750600082115b1561047c5781600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a381600160005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505403925050819055506001905061048656610485565b60009050610486565b5b9392505050565b6000600160005060008373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505490506104c6565b919050565b600081600160005060003373ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561050c5750600082115b156105fb5781600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a36001905061060556610604565b60009050610605565b5b92915050565b6000600260005060008473ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005054905061066e565b92915050565b60006000600050549050610683565b9056";

        public StandardTokenDeployment() : base(BYTECODE) { }

        [Parameter("uint256", "totalSupply")]
        public BigInteger TotalSupply { get; set; }
    }
}
