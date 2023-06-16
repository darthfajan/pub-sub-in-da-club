using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    public interface IChannelClient : IDisposable
    {
        bool Subscribe(string channel, IChannelSubscriber messageReceived);
        bool Publish(string channel, string message);
    }
}
