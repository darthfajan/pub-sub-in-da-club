using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Shared.TCP
{
    public class TCPMessageParser : IMessageParser
    {
        public const char PublishEncoding = 'P';
        public const char SubscribeEncoding = 'S';
        public const char ContentEncoding = 'C';
        public const char ACKEncoding = 'A';
        public const char ErrorEncoding = 'E';
        public const char EncodingSeparator = '|';
        public const string EndEncoodingTerminator = "\r\n";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string CreatePublishMessage(string channel, string message)
        {
            if(string.IsNullOrEmpty(message))
                return string.Empty;

            if (string.IsNullOrEmpty(channel))
                return string.Empty;

            return $"{PublishEncoding}{EncodingSeparator}{channel.ToUpperInvariant()}{EncodingSeparator}{message}{EndEncoodingTerminator}";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string CreateSubscribeMessage(string channel)
        {
            if (string.IsNullOrEmpty(channel))
                return string.Empty;

            return $"{SubscribeEncoding}{EncodingSeparator}{channel.ToUpperInvariant()}{EncodingSeparator}{EndEncoodingTerminator}";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string CreateContentMessage(string channel, string message)
        {
            return $"{ContentEncoding}{EncodingSeparator}{channel.ToUpperInvariant()}{EncodingSeparator}{message}{EndEncoodingTerminator}";
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string CreateAckMessage() => $"{ACKEncoding}{EncodingSeparator}{EncodingSeparator}{EndEncoodingTerminator}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string CreateErrorMessage() => $"{ErrorEncoding}{EncodingSeparator}{EncodingSeparator}{EndEncoodingTerminator}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IMessageInfo Decode(string encodedMessage)
        {
            // extract the type
            var separatorIndex = encodedMessage.IndexOf(EncodingSeparator);
            if(separatorIndex < 0)
                return null;

            string messageType = encodedMessage.Substring(0, separatorIndex);

            // extract the channel
            var channelMessage = encodedMessage.Substring(separatorIndex + 1);
            separatorIndex = channelMessage.IndexOf(EncodingSeparator);

            if (separatorIndex < 0)
                return null;

            var channel = channelMessage.Substring(0, separatorIndex);
            // extract the message
            var message = channelMessage.Substring(separatorIndex + 1);

            int terminatorIndex = message.IndexOf(EndEncoodingTerminator);
            if (terminatorIndex < 0)
                return null;

            message = message.Substring(0, terminatorIndex);

            return new TCPMessageInfo()
            {
                MessageType = messageType.Decoded(),
                Channel = channel,
                Content = message 
            };

        }
    }
}
