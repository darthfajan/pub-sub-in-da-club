using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    /// <summary>
    /// Allows to personalize the configuration of the client
    /// </summary>
    public interface IClientConfigurationSettings
    {
        /// <summary>
        /// The host of the server
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// The connection port to the server
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// Eventual logger for debug purpose
        /// </summary>
        IPubSubLogger Logger { get; set; }
        /// <summary>
        /// Exchanged messages parser
        /// </summary>
        IMessageParser Parser { get; set; }
    }
}
