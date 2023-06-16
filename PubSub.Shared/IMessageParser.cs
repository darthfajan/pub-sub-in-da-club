using PubSub.Shared.TCP;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Shared
{
    public interface IMessageParser
    {
        /// <summary>
        /// Return the encoded string for a subscription to a channel
        /// </summary>
        /// <param name="channel">name of the channel to subscribe to (case insensitive)</param>
        /// <returns>encoded message string</returns>
        string CreateSubscribeMessage(string channel);
        /// <summary>
        /// Return the encoded string for a publication of a message to a channel
        /// </summary>
        /// <param name="channel">Name of the channel (case insensitive)</param>
        /// <param name="message">Content of the message</param>
        /// <returns>encoded message string</returns>
        string CreatePublishMessage(string channel, string message);
        /// <summary>
        /// Return the encoded string for a dispatching of a content via a channel
        /// </summary>
        /// <param name="channel">Name of the channel (case insensitive)</param>
        /// <param name="message">Content of the message</param>
        /// <returns>encoded message string</returns>
        string CreateContentMessage(string channel, string message);
        /// <summary>
        /// Created the encoded string of an ack message
        /// </summary>
        /// <returns>encoded message string</returns>
        string CreateAckMessage();
        /// <summary>
        /// Created the encoded string of an error message
        /// </summary>
        /// <returns>encoded message string</returns>
        string CreateErrorMessage();
        /// <summary>
        /// Decodes an encoded message to its specific info
        /// </summary>
        /// <param name="encodedMessage">the sent message</param>
        /// <returns>the info about the encoded message</returns>
        IMessageInfo Decode(string encodedMessage);
    }
}
