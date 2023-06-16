using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    public interface IClientConfigurationSettings
    {
        string Host { get; set; }
        int Port { get; set; }
        IPubSubLogger Logger { get; set; }
    }
}
