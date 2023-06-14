using FluentAssertions;
using PubSub.Shared.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Publish_message_should_be_created_correctly()
        {
            var message = "test message";
            var channel = "testChannel";

            string encodedMessage = TCPMessageParser.CreatePublishMessage(channel, message);
            encodedMessage.Should().Be($"{TCPMessageParser.PublishEncoding}{TCPMessageParser.EncodingSeparator}{channel.ToUpperInvariant()}{TCPMessageParser.EncodingSeparator}{message}{TCPMessageParser.EndEncoodingTerminator}"); // P|CHANNEL|message\r\n
        }

        [TestMethod]
        public void Publish_empty_or_null_message_should_be_empty()
        {
            var channel = "testChannel";

            string encodedMessage = TCPMessageParser.CreatePublishMessage(channel, null);
            encodedMessage.Should().Be(string.Empty);
            encodedMessage = TCPMessageParser.CreatePublishMessage(channel, string.Empty);
            encodedMessage.Should().Be(string.Empty);
        }

        [TestMethod]
        public void Publish_empty_or_null_channel_should_be_empty()
        {
            var message = "test";

            string encodedMessage = TCPMessageParser.CreatePublishMessage(null, message);
            encodedMessage.Should().Be(string.Empty);
            encodedMessage = TCPMessageParser.CreatePublishMessage(string.Empty, message);
            encodedMessage.Should().Be(string.Empty);
        }

        [TestMethod]
        public void Publish_message_should_be_parsed_correctly()
        {
            var message = "test message";
            var channel = "testChannel";
            var encodedMessage = $"{TCPMessageParser.PublishEncoding}{TCPMessageParser.EncodingSeparator}{channel.ToUpperInvariant()}{TCPMessageParser.EncodingSeparator}{message}{TCPMessageParser.EndEncoodingTerminator}";

            MessageInfo decodedMessage = TCPMessageParser.Decode(encodedMessage);
            decodedMessage.MessageType.Should().Be(MessageType.Publish); 
            decodedMessage.Message.Should().Be(message); 
            decodedMessage.Channel.Should().Be(channel.ToUpperInvariant()); 
        }

        [TestMethod]
        public void Invalid_message_should_be_parsed_correctly()
        {
            var message = "test message";
            var encodedMessage = $"{TCPMessageParser.PublishEncoding}{TCPMessageParser.EncodingSeparator}{message}{TCPMessageParser.EndEncoodingTerminator}";

            MessageInfo decodedMessage = TCPMessageParser.Decode(encodedMessage);
            decodedMessage.Should().BeNull();
        }

        [TestMethod]
        public void Subscribe_message_should_be_created_correctly()
        {
            var channel = "testChannel";

            string encodedMessage = TCPMessageParser.CreateSubscribeMessage(channel);
            encodedMessage.Should().Be($"{TCPMessageParser.SubscribeEncoding}{TCPMessageParser.EncodingSeparator}{channel.ToUpperInvariant()}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EndEncoodingTerminator}"); // S|CHANNEL|\r\n
        }

        [TestMethod]
        public void Subscribe_message_should_be_parsed_correctly()
        {
            var channel = "testChannel";
            var encodedMessage = $"{TCPMessageParser.SubscribeEncoding}{TCPMessageParser.EncodingSeparator}{channel.ToUpperInvariant()}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EndEncoodingTerminator}";

            MessageInfo decodedMessage = TCPMessageParser.Decode(encodedMessage);
            decodedMessage.MessageType.Should().Be(MessageType.Subscribe);
            decodedMessage.Message.Should().Be(string.Empty);
            decodedMessage.Channel.Should().Be(channel.ToUpperInvariant());
        }

        [TestMethod]
        public void Content_message_should_be_created_correctly()
        {
            var message = "test message";
            var channel = "testChannel";

            string encodedMessage = TCPMessageParser.CreateContentMessage(channel, message);
            encodedMessage.Should().Be($"{TCPMessageParser.ContentEncoding}{TCPMessageParser.EncodingSeparator}{channel.ToUpperInvariant()}{TCPMessageParser.EncodingSeparator}{message}{TCPMessageParser.EndEncoodingTerminator}"); // C|CHANNEL|message\r\n
        }

        [TestMethod]
        public void Content_message_should_be_parsed_correctly()
        {
            var message = "test message";
            var channel = "testChannel";
            var encodedMessage = $"{TCPMessageParser.ContentEncoding}{TCPMessageParser.EncodingSeparator}{channel.ToUpperInvariant()}{TCPMessageParser.EncodingSeparator}{message}{TCPMessageParser.EndEncoodingTerminator}";

            MessageInfo decodedMessage = TCPMessageParser.Decode(encodedMessage);
            decodedMessage.MessageType.Should().Be(MessageType.Content);
            decodedMessage.Message.Should().Be(message);
            decodedMessage.Channel.Should().Be(channel.ToUpperInvariant());
        }

        [TestMethod]
        public void Ack_message_should_be_created_correctly()
        {
            string encodedMessage = TCPMessageParser.CreateAckMessage();
            encodedMessage.Should().Be($"{TCPMessageParser.ACKEncoding}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EndEncoodingTerminator}"); // A||\r\n
        }

        [TestMethod]
        public void Ack_message_should_be_parsed_correctly()
        {
            var encodedMessage = $"{TCPMessageParser.ACKEncoding}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EndEncoodingTerminator}";

            MessageInfo decodedMessage = TCPMessageParser.Decode(encodedMessage);
            decodedMessage.MessageType.Should().Be(MessageType.Ack);
            decodedMessage.Message.Should().Be(string.Empty);
            decodedMessage.Channel.Should().Be(string.Empty);
        }

        [TestMethod]
        public void Error_message_should_be_created_correctly()
        {
            string encodedMessage = TCPMessageParser.CreateErrorMessage();
            encodedMessage.Should().Be($"{TCPMessageParser.ErrorEncoding}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EndEncoodingTerminator}"); // A||\r\n
        }

        [TestMethod]
        public void Error_message_should_be_parsed_correctly()
        {
            var encodedMessage = $"{TCPMessageParser.ErrorEncoding}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EncodingSeparator}{TCPMessageParser.EndEncoodingTerminator}";

            MessageInfo decodedMessage = TCPMessageParser.Decode(encodedMessage);
            decodedMessage.MessageType.Should().Be(MessageType.Error);
            decodedMessage.Message.Should().Be(string.Empty);
            decodedMessage.Channel.Should().Be(string.Empty);
        }
    }
}
