using PubSub.Shared;
using PubSub.Shared.TCP;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client.TCPClient
{
    internal class BasicTCPClientConfiguration : IClientConfigurationSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = ConnectionDefaults.StandardPort;
        public IPubSubLogger Logger { get; set; }
        public IMessageParser Parser { get; set; } = new TCPMessageParser();
    }
}
