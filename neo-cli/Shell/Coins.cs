using DbgViewTR;
using Neo.Core;
using Neo.Network;
using Neo.SmartContract;
using Neo.Wallets;
using System;
using System.Linq;

namespace Neo.Shell
{

    public class Coins
    {
        private Wallet current_wallet;
        private LocalNode local_node;

        public Coins(Wallet wallet, LocalNode node)
        {
            TR.enter();
            current_wallet = wallet;
            local_node = node;
            TR.exit();
        }

        public Fixed8 UnavailableBonus()
        {
            TR.enter();
            uint height = Blockchain.Default.Height + 1;
            Fixed8 unavailable;

            try
            {
                unavailable = Blockchain.CalculateBonus(current_wallet.FindUnspentCoins().Where(p => p.Output.AssetId.Equals(Blockchain.GoverningToken.Hash)).Select(p => p.Reference), height);
            }
            catch (Exception)
            {
                unavailable = Fixed8.Zero;
            }

            return TR.exit(unavailable);
        }


        public Fixed8 AvailableBonus()
        {
            TR.enter();
            return TR.exit(Blockchain.CalculateBonus(current_wallet.GetUnclaimedCoins().Select(p => p.Reference)));
        }


        public ClaimTransaction Claim()
        {
            TR.enter();

            if (this.AvailableBonus() == Fixed8.Zero)
            {
                Console.WriteLine($"no gas to claim");
                return TR.exit((ClaimTransaction) null);
            }

            CoinReference[] claims = current_wallet.GetUnclaimedCoins().Select(p => p.Reference).ToArray();
            if (claims.Length == 0) return TR.exit((ClaimTransaction)null);

            ClaimTransaction tx = new ClaimTransaction
            {
                Claims = claims,
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = Blockchain.UtilityToken.Hash,
                        Value = Blockchain.CalculateBonus(claims),
                        ScriptHash = current_wallet.GetChangeAddress()
                    }
                }

            };

            return TR.exit((ClaimTransaction)SignTransaction(tx));
        }


        private Transaction SignTransaction(Transaction tx)
        {
            TR.enter();
            if (tx == null)
            {
                Console.WriteLine($"no transaction specified");
                return TR.exit((Transaction)null);
            }
            ContractParametersContext context;

            try
            {
                context = new ContractParametersContext(tx);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"unsynchronized block");

                return TR.exit((Transaction)null);
            }

            current_wallet.Sign(context);

            if (context.Completed)
            {
                context.Verifiable.Scripts = context.GetScripts();
                current_wallet.ApplyTransaction(tx);

                bool relay_result = local_node.Relay(tx);

                if (relay_result)
                {
                    return TR.exit(tx);
                }
                else
                {
                    Console.WriteLine($"Local Node could not relay transaction: {tx.Hash.ToString()}");
                }
            }
            else
            {
                Console.WriteLine($"Incomplete Signature: {context.ToString()}");
            }

            return TR.exit((Transaction)null);
        }
    }
}
