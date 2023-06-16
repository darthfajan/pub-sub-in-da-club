using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server
{
    /// <summary>
    /// Allowa the server configuration
    /// </summary>
    public interface IChannelServerConfiguration
    {
        /// <summary>
        /// Port opened by the server
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// Eventual logger for debug purpose
        /// </summary>
        IPubSubLogger Logger { get; set; }
        /// <summary>
        /// Exchanged message parser
        /// </summary>
        IMessageParser Parser { get; set; }
    }
}
