using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    public interface IChannelSubscriber
    {
        void OnChannelMessage(string channel, IMessage message);
    }
}
