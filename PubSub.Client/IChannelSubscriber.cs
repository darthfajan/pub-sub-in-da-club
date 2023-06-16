using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    public interface IChannelSubscriber
    {
        void OnMessage(string channel, string message);
        void OnClose(string channel);
    }
}
