using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Shared
{
    public interface IMessageInfo
    {
        /// <summary>
        /// The content of the message that was sent
        /// </summary>
        string Content { get; set; }
        /// <summary>
        /// The channel of the message
        /// </summary>
        string Channel { get; set; }
        /// <summary>
        /// The type of message sent (i.e. ack, error, subscription, publish, content)
        /// </summary>
        MessageType MessageType { get; set; }
    }
}
