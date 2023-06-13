using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server
{
    public interface IChannelServerConfiguration
    {
        int Port { get; set; }
        IPubSubLogger Logger { get; set; }
    }
}
