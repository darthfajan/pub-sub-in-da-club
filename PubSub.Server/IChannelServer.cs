using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server
{
    public interface IChannelServer : IDisposable
    {
        void Init();
    }
}
