using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server.TCPServer
{
    internal class BasicTCPServerConfiguration : IChannelServerConfiguration
    {
        public const int StandardPort = 6666;

        public int Port { get; set; } = BasicTCPServerConfiguration.StandardPort;
        public IPubSubLogger Logger { get; set; }
    }
}
