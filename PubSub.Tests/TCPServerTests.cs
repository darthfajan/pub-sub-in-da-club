using FluentAssertions;
using Moq;
using PubSub.Server;
using PubSub.Server.TCPServer;
using PubSub.Shared;
using PubSub.Shared.TCP;
using System.Net.Sockets;
using System.Text;

namespace PubSub.Tests
{
    [TestClass]
    public class TCPServerTests
    {
        private const string TestHost = "127.0.0.1";
        private IPubSubLogger _logger;
        private TCPServer _server;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Init()
        {
            _logger = new TestLogger(TestContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server?.Dispose();
        }

        [TestMethod]
        public void TCPServer_should_be_created_without_logger_and_standard_settings()
        {
            // setup
            _server = new TCPServer();
            _server.Init();
            // execute
            TcpClient client = new TcpClient();
            client.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            client.Connected.Should().BeTrue();
        }

        [TestMethod]
        public void TCPServer_should_be_created_with_a_configuration()
        {
            // setup
            var newPort = 4444;
            _server = new TCPServer(cfg => cfg.Port = newPort);
            _server.Init();

            // execute
            TcpClient client = new TcpClient();
            client.Connect(TestHost, newPort);
            // check
            client.Connected.Should().BeTrue();
        }

        [TestMethod]
        public void Logger_should_be_correctly_called()
        {
            // setup
            bool loggerCalled = false;
            var loggerMoq = new Mock<IPubSubLogger>();
            loggerMoq.Setup(p => p.Info(It.IsAny<string>())).Callback(() => loggerCalled = true);
            // execute
            _server = new TCPServer(cfg =>
            {
                cfg.Logger = loggerMoq.Object;
            });
            _server.Init();
            // check
            loggerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void A_publish_message_should_create_the_correct_channel()
        {
            // execute
            _server = new TCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            client.Connected.Should().BeTrue();
            var channel = "TEST";
            var sendingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreatePublishMessage(channel, "test"));
            client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);

            Thread.Sleep(1000);
            _server._channels.Keys.Should().HaveCount(1);
        }

        [TestMethod]
        public void A_subscription_message_should_create_the_correct_channel_and_receive_the_publish()
        {
            // execute
            _server = new TCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            client.Connected.Should().BeTrue();
            var channel = "TEST";
            var sendingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreateSubscribeMessage(channel));
            client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);

            Thread.Sleep(1000);
            _server._channels.Keys.Should().HaveCount(1);

            TcpClient publishClient = new TcpClient();
            publishClient.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            publishClient.Connected.Should().BeTrue();
            var publishingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreatePublishMessage(channel, "test"));
            publishClient.GetStream().Write(publishingBytes, 0, publishingBytes.Length);

            Thread.Sleep(1000);
            client.ReceiveBufferSize.Should().BeGreaterThan(0);
            byte[] received = new byte[client.ReceiveBufferSize];
            client.GetStream().Read(received, 0, client.ReceiveBufferSize);
            string result = Encoding.UTF8.GetString(received);
            result = result.Substring(0, result.IndexOf('\0'));
            result.Length.Should().BeGreaterThan(0);
            _logger.Info($"Received {result}");
        }

        [TestMethod]
        public void Multiple_subscriptions_should_receive_the_same_publish()
        {
            // execute
            _server = new TCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            client.Connected.Should().BeTrue();
            var channel = "TEST";
            var sendingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreateSubscribeMessage(channel));
            client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);

            TcpClient client2 = new TcpClient();
            client2.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            client2.Connected.Should().BeTrue();
            client2.GetStream().Write(sendingBytes, 0, sendingBytes.Length);

            Thread.Sleep(1000);
            _server._channels.Keys.Should().HaveCount(1);

            TcpClient publishClient = new TcpClient();
            publishClient.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            publishClient.Connected.Should().BeTrue();
            var publishingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreatePublishMessage(channel, "test"));
            publishClient.GetStream().Write(publishingBytes, 0, publishingBytes.Length);

            Thread.Sleep(1000);
            client.ReceiveBufferSize.Should().BeGreaterThan(0);
            byte[] received = new byte[client.ReceiveBufferSize];
            client.GetStream().Read(received, 0, client.ReceiveBufferSize);
            string result = Encoding.UTF8.GetString(received);
            result = result.Substring(0, result.IndexOf('\0'));
            result.Length.Should().BeGreaterThan(0);
            _logger.Info($"Received {result} for client 1");

            client2.ReceiveBufferSize.Should().BeGreaterThan(0);
            received = new byte[client2.ReceiveBufferSize];
            client2.GetStream().Read(received, 0, client2.ReceiveBufferSize);
            result = Encoding.UTF8.GetString(received);
            result = result.Substring(0, result.IndexOf('\0'));
            result.Length.Should().BeGreaterThan(0);
            _logger.Info($"Received {result} for client 2");
        }

        [TestMethod]
        public void Multiple_subscriptions_of_the_same_client_should_be_possible()
        {
            // execute
            _server = new TCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.Connect(TestHost, TCPServerConfiguration.StandardPort);
            // check
            client.Connected.Should().BeTrue();
            var channel = "TEST";
            var sendingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreateSubscribeMessage(channel));
            client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);

            sendingBytes = Encoding.UTF8.GetBytes(TCPMessageParser.CreateSubscribeMessage("SECONDTEST"));
            client.GetStream().Write(sendingBytes, 0, sendingBytes.Length);

            Thread.Sleep(1000);
            _server._channels.Keys.Should().HaveCount(2);
        }

        // TODO:
        // test disconnection
        // test big message
        // test client
        // Integration tests
        // Refactoring
        // Comments
        // Docs

        // if there is time, grpc (only integration)

    }
}