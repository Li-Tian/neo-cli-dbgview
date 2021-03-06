﻿using NoDbgViewTR;
using Microsoft.Extensions.Configuration;
using Neo.Network;

namespace Neo
{
    internal class Settings
    {
        public PathsSettings Paths { get; }
        public P2PSettings P2P { get; }
        public RPCSettings RPC { get; }
        public UnlockWalletSettings UnlockWallet { get; set; }

        public static Settings Default { get; }

        static Settings()
        {
            TR.Enter();
            IConfigurationSection section = new ConfigurationBuilder().AddJsonFile("config.json").Build().GetSection("ApplicationConfiguration");
            Default = new Settings(section);
            TR.Exit();
        }

        public Settings(IConfigurationSection section)
        {
            TR.Enter();
            this.Paths = new PathsSettings(section.GetSection("Paths"));
            this.P2P = new P2PSettings(section.GetSection("P2P"));
            this.RPC = new RPCSettings(section.GetSection("RPC"));
            this.UnlockWallet = new UnlockWalletSettings(section.GetSection("UnlockWallet"));
            TR.Exit();
        }
    }

    internal class PathsSettings
    {
        public string Chain { get; }
        public string ApplicationLogs { get; }

        public PathsSettings(IConfigurationSection section)
        {
            TR.Enter();
            this.Chain = string.Format(section.GetSection("Chain").Value, Message.Magic.ToString("X8"));
            this.ApplicationLogs = string.Format(section.GetSection("ApplicationLogs").Value, Message.Magic.ToString("X8"));
            TR.Exit();
        }
    }

    internal class P2PSettings
    {
        public ushort Port { get; }
        public ushort WsPort { get; }

        public P2PSettings(IConfigurationSection section)
        {
            TR.Enter();
            this.Port = ushort.Parse(section.GetSection("Port").Value);
            this.WsPort = ushort.Parse(section.GetSection("WsPort").Value);
            TR.Exit();
        }
    }

    internal class RPCSettings
    {
        public ushort Port { get; }
        public string SslCert { get; }
        public string SslCertPassword { get; }

        public RPCSettings(IConfigurationSection section)
        {
            TR.Enter();
            this.Port = ushort.Parse(section.GetSection("Port").Value);
            this.SslCert = section.GetSection("SslCert").Value;
            this.SslCertPassword = section.GetSection("SslCertPassword").Value;
            TR.Exit();
        }
    }

    public class UnlockWalletSettings
    {
        public string Path { get; }
        public string Password { get; }
        public bool StartConsensus { get; }
        public bool IsActive { get; }

        public UnlockWalletSettings(IConfigurationSection section)
        {
            TR.Enter();
            if (section.Exists())
            {
                this.Path = section.GetSection("Path").Value;
                this.Password = section.GetSection("Password").Value;
                this.StartConsensus = bool.Parse(section.GetSection("StartConsensus").Value);
                this.IsActive = bool.Parse(section.GetSection("IsActive").Value);
            }
            TR.Exit();
        }
    }
}
