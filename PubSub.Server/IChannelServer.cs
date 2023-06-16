using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server
{
    /// <summary>
    /// The PubSub server interface
    /// </summary>
    public interface IChannelServer : IDisposable
    {
        /// <summary>
        /// Initializes the server communications
        /// </summary>
        void Init();
    }
}
