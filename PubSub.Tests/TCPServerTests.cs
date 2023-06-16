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
        private string _messageContent = "MessageContent";
        private const string TestHost = "127.0.0.1";
        private IPubSubLogger _logger;
        private BasicTCPServer _server;

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
            _server = new BasicTCPServer();
            _server.Init();
            // execution
            TcpClient client = new TcpClient();
            // verification
            client.CheckConnection(TestHost);
        }

        [TestMethod]
        public void TCPServer_should_be_created_with_a_configuration()
        {
            // setup
            var newPort = 4444;
            _server = new BasicTCPServer(cfg => cfg.Port = newPort);
            _server.Init();

            // execution
            TcpClient client = new TcpClient();
            // verification
            client.CheckConnection(TestHost, newPort);
        }

        [TestMethod]
        public void Logger_should_be_correctly_called()
        {
            // setup
            bool loggerCalled = false;
            var loggerMoq = new Mock<IPubSubLogger>();
            loggerMoq.Setup(p => p.Info(It.IsAny<string>())).Callback(() => loggerCalled = true);
            // execution
            _server = new BasicTCPServer(cfg =>
            {
                cfg.Logger = loggerMoq.Object;
            });
            _server.Init();
            // verification
            loggerCalled.Should().BeTrue();
        }

        [TestMethod]
        public void A_publish_message_should_create_the_correct_channel()
        {
            // setup
            var channel = "TEST";
            _server = new BasicTCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            TcpClient client = new TcpClient();
            client.CheckConnection(TestHost);

            // execution
            client.SendPublish(channel, _messageContent);

            // verification
            client.CheckAck();
            _server._channels.Keys.Should().HaveCount(1);
        }


        [TestMethod]
        public void A_subscription_message_should_create_the_correct_channel_and_receive_the_publish()
        {
            // setup
            _server = new BasicTCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.CheckConnection(TestHost);
            // check
            var channel = "TEST";
            client.SendSubscription(channel);
            client.CheckAck();
            _server._channels.Keys.Should().HaveCount(1);

            TcpClient publishClient = new TcpClient();
            publishClient.CheckConnection(TestHost);

            publishClient.SendPublish(channel, _messageContent);
            publishClient.CheckAck();

            client.CheckContent(_messageContent);
        }


        [TestMethod]
        public void Multiple_subscriptions_should_receive_the_same_publish()
        {
            // execute
            var channel = "TEST";
            _server = new BasicTCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.CheckConnection(TestHost);
            client.SendSubscription(channel);

            TcpClient client2 = new TcpClient();
            client2.CheckConnection(TestHost);
            client2.SendSubscription(channel);

            client.CheckAck();
            client2.CheckAck();
            _server._channels.Keys.Should().HaveCount(1);
            _server._channels.Values.First().Should().HaveCount(2);

            TcpClient publishClient = new TcpClient();
            publishClient.CheckConnection(TestHost);

            publishClient.SendPublish(channel, _messageContent);
            publishClient.CheckAck();

            client.CheckContent(_messageContent);
            client2.CheckContent(_messageContent);

        }

        [TestMethod]
        public void Multiple_subscriptions_of_the_same_client_should_be_possible()
        {
            // execute
            var channel = "TEST";
            var secondChannel = "SECONDTEST";
            _server = new BasicTCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.CheckConnection(TestHost);
            client.SendSubscription(channel);

            client.CheckAck();

            client.SendSubscription(secondChannel);
            client.CheckAck();

            _server._channels.Keys.Should().HaveCount(2);
        }

        [TestMethod]
        public void Client_can_be_subscribed_two_times_same_channel_without_repetition_on_clients_list()
        {
            // execute
            var channel = "TEST";
            _server = new BasicTCPServer(cfg =>
            {
                cfg.Logger = _logger;
            });
            _server.Init();
            // check
            TcpClient client = new TcpClient();
            client.CheckConnection(TestHost);
            client.SendSubscription(channel);

            client.CheckAck();

            client.SendSubscription(channel);
            client.CheckAck();

            _server._channels.Keys.Should().HaveCount(1);
            _server._channels.Values.First().Should().HaveCount(1);
        }

        [TestMethod]
        public void Server_factory_should_create_a_valid_server()
        {
            // setup
            using var server = ChannelServerFactory.CreateServer();
            server.Should().BeOfType(typeof(BasicTCPServer));
            server.Init();
            // execution
            TcpClient client = new TcpClient();
            // verification
            client.CheckConnection(TestHost);
        }

        [TestMethod]
        public void Server_factory_should_create_a_valid_server_with_a_config()
        {
            // setup
            bool loggerCalled = false;
            var loggerMoq = new Mock<IPubSubLogger>();
            loggerMoq.Setup(p => p.Info(It.IsAny<string>())).Callback(() => loggerCalled = true);
            using var server = ChannelServerFactory.CreateServer(configurationAction: cfg =>
            {
                cfg.Logger = loggerMoq.Object;
            });
            server.Should().BeOfType(typeof(BasicTCPServer));
            server.Init();
            // execution
            TcpClient client = new TcpClient();
            // verification
            client.CheckConnection(TestHost);
            loggerCalled.Should().BeTrue();

        }

        // TODO:
        // test client
        // Integration tests
        // Refactoring
        // Comments
        // Docs

        // if there is time, grpc (only integration)


    }
}