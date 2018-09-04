using NoDbgViewTR;
using Neo.Network;
using Neo.Wallets;
using System;
using System.IO;

namespace Neo.Consensus
{
    internal class ConsensusWithLog : ConsensusService
    {
        private string log_dictionary;

        public ConsensusWithLog(LocalNode localNode, Wallet wallet, string log_dictionary)
            : base(localNode, wallet)
        {
            TR.Enter();
            this.log_dictionary = log_dictionary;
            TR.Exit();
        }

        protected override void Log(string message)
        {
            TR.Enter();
            DateTime now = DateTime.Now;
            string line = $"[{now.TimeOfDay:hh\\:mm\\:ss}] {message}";
            Console.WriteLine(line);
            if (string.IsNullOrEmpty(log_dictionary)) return;
            lock (log_dictionary)
            {
                Directory.CreateDirectory(log_dictionary);
                string path = Path.Combine(log_dictionary, $"{now:yyyy-MM-dd}.log");
                File.AppendAllLines(path, new[] { line });
            }
            TR.Exit();
        }
    }
}
