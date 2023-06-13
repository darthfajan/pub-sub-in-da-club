using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    public interface IChannelClient
    {
        bool Subscribe(string channel, IChannelSubscriber subscriber);
        void Publish(string channel, IMessage message);
    }
}
