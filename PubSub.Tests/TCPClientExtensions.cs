using FluentAssertions;
using PubSub.Server.TCPServer;
using PubSub.Shared.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.Tests
{
    internal static class TCPClientExtensions
    {
        public static void CheckAck(this TcpClient client)
        {
            client.ReceiveBufferSize.Should().BeGreaterThan(0);
            var received = new byte[client.ReceiveBufferSize];
            client.GetStream().Read(received, 0, client.ReceiveBufferSize);

            var receivedString = Encoding.UTF8.GetString(received);
            var decodedMessage = TCPMessageParser.Decode(receivedString);
            decodedMessage.MessageType.Should().Be(MessageType.Ack);
        }

        public static void SendPublish(this TcpClient client, string channel, string content)
        {
            var sendingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreatePublishMessage(channel, content));
            client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);
        }


        public static void CheckConnection(this TcpClient client, string host, int port = BasicTCPServerConfiguration.StandardPort)
        {
            client.Connect(host, port);
            client.Connected.Should().BeTrue();
        }

        public static void SendSubscription(this TcpClient client, string channel)
        {
            var sendingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreateSubscribeMessage(channel));
            client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);
        }

        public static void CheckContent(this TcpClient client, string content)
        {
            client.ReceiveBufferSize.Should().BeGreaterThan(0);
            byte[] received = new byte[client.ReceiveBufferSize];
            client.GetStream().Read(received, 0, client.ReceiveBufferSize);
            var receivedString = Encoding.UTF8.GetString(received);
            var decodedMessage = TCPMessageParser.Decode(receivedString);
            decodedMessage.MessageType.Should().Be(MessageType.Content);
            decodedMessage.Message.Should().Be(content);
        }
    }
}
