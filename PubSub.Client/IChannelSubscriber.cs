using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Client
{
    /// <summary>
    /// The interface for a channel subscription
    /// </summary>
    public interface IChannelSubscriber
    {
        /// <summary>
        /// This is called when a new message for the channel arrives
        /// </summary>
        /// <param name="channel">The channel of the message: this is case insensitive</param>
        /// <param name="message">The message published on the channel</param>
        void OnMessage(string channel, string message);
        /// <summary>
        /// Called when the server closes the connection
        /// </summary>
        /// <param name="channel">Name of the channel closed</param>
        void OnClose(string channel);
    }
}
