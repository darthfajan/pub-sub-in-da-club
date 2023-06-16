using System;
using System.Collections.Generic;
using System.Text;
using PubSub.Shared.TCP;

namespace PubSub.Shared
{
    public enum MessageType
    {
        None,
        Publish,
        Subscribe,
        Content,
        Ack,
        Error
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
                case MessageType.Ack:
                    return TCPMessageParser.ACKEncoding;
                case MessageType.Error:
                    return TCPMessageParser.ErrorEncoding;
            }

            throw new ArgumentException("Invalid value", nameof(messageType));
        }

        public static MessageType Decoded(this string encodedType)
        {
            if (encodedType.Equals(TCPMessageParser.PublishEncoding.ToString()))
                return MessageType.Publish;
            if (encodedType.Equals(TCPMessageParser.SubscribeEncoding.ToString()))
                return MessageType.Subscribe;
            if (encodedType.Equals(TCPMessageParser.ContentEncoding.ToString()))
                return MessageType.Content;
            if (encodedType.Equals(TCPMessageParser.ACKEncoding.ToString()))
                return MessageType.Ack;
            if (encodedType.Equals(TCPMessageParser.ErrorEncoding.ToString()))
                return MessageType.Error;

            return MessageType.None;
        }
    }
}

