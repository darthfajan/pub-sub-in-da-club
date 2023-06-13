using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server.TCPServer
{
    internal class TCPServerConfiguration : IChannelServerConfiguration
    {
        public const int StandardPort = 6666;

        public int Port { get; set; } = TCPServerConfiguration.StandardPort;
        public IPubSubLogger Logger { get; set; }
    }
}
