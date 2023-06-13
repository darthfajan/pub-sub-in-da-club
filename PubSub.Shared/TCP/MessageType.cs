using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Shared.TCP
{
    public enum MessageType
    {
        None,
        Publish,
        Subscribe,
        Content
    }
    public static class MessageTypeExtensions
    {
        public static char Encoded(this MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Publish:
                    return TCPMessageParser.PublishEncoding;
                case MessageType.Subscribe:
                    return TCPMessageParser.SubscribeEncoding;
                case MessageType.Content:
                    return TCPMessageParser.ContentEncoding;
            }

            throw new ArgumentException("Invalid value", nameof(messageType));
        }

        public static MessageType Decoded(this string encodedType)
        {
            if(encodedType.Equals(TCPMessageParser.PublishEncoding.ToString()))
                    return MessageType.Publish;
            if (encodedType.Equals(TCPMessageParser.SubscribeEncoding.ToString()))
                return MessageType.Subscribe;
            if (encodedType.Equals(TCPMessageParser.ContentEncoding.ToString()))
                return MessageType.Content;

            return MessageType.None;
        }
    }
}

