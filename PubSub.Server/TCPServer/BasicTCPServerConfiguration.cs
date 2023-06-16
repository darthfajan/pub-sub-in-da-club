using PubSub.Shared;
using PubSub.Shared.TCP;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server.TCPServer
{
    internal class BasicTCPServerConfiguration : IChannelServerConfiguration
    {
        public int Port { get; set; } = ConnectionDefaults.StandardPort;
        public IPubSubLogger Logger { get; set; }
        public IMessageParser Parser { get; set; } = new TCPMessageParser();
    }
}
