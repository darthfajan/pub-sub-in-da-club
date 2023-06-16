using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    /// <summary>
    /// The inferface for a PubSub client
    /// </summary>
    public interface IChannelClient : IDisposable
    {
        /// <summary>
        /// Allows to subscribe to an argument channel to be notified of its published messages
        /// </summary>
        /// <param name="channel">The name of the channel to be subscribed to. It is case insensitive!</param>
        /// <param name="subscriber">The subscriber to the channel</param>
        /// <returns></returns>
        bool Subscribe(string channel, IChannelSubscriber subscriber);
        /// <summary>
        /// Allows to publish a message to a specific channel
        /// </summary>
        /// <param name="channel">The name of the channel</param>
        /// <param name="message">The message that must be published</param>
        /// <returns></returns>
        bool Publish(string channel, string message);
    }
}
