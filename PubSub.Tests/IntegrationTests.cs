using FluentAssertions;
using Moq;
using PubSub.Client;
using PubSub.Server;
using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private IPubSubLogger _logger;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Init()
        {
            _logger = new TestLogger(TestContext);
        }

        [TestMethod]
        public void An_example_of_a_pub_sub_environment()
        {
            // Creating the server
            using var server = ChannelServerFactory.CreateServer(configurationAction: cfg => cfg.Logger = _logger );

            // creating the first client
            using var client1 = ChannelClientFactory.CreateClient(configurationAction: cfg => cfg.Logger = _logger);
            var client1SubscriberMoq = new Mock<IChannelSubscriber>();
            string client1ChannelArrived = string.Empty;
            string client1ContentArrived = string.Empty;
            ManualResetEvent messageArrivedOnClient1 = new ManualResetEvent(false);
            client1SubscriberMoq.Setup(x => x.OnMessage(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((channel, content) =>
            {
                client1ChannelArrived = channel;
                client1ContentArrived = content;
                messageArrivedOnClient1.Set();
            });

            // and the second
            using var client2 = ChannelClientFactory.CreateClient();
            var client2SubscriberMoq = new Mock<IChannelSubscriber>();
            string client2ChannelArrived = string.Empty;
            string client2ContentArrived = string.Empty;
            ManualResetEvent messageArrivedOnClient2 = new ManualResetEvent(false);
            client2SubscriberMoq.Setup(x => x.OnMessage(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((channel, content) =>
            {
                client2ChannelArrived = channel;
                client2ContentArrived = content;
                messageArrivedOnClient2.Set();
            });

            var argument1 = "Argument1";
            var argument2 = "Argument2";
            var commonArgument = "CommonArgument";

            // starting the server
            server.Init();

            // subscribe to channels
            client1.Subscribe(argument1, client1SubscriberMoq.Object);
            client1.Subscribe(commonArgument, client1SubscriberMoq.Object);
            client2.Subscribe(argument2, client2SubscriberMoq.Object);
            client2.Subscribe(commonArgument, client2SubscriberMoq.Object);

            // the first message should arrive only to the first client
            var firstMessage = "The first message";
            client2.Publish(argument1, firstMessage);
            messageArrivedOnClient1.WaitOne(1000);
            client1ContentArrived.Should().Be(firstMessage);
            messageArrivedOnClient2.WaitOne(1000); // this will slow down the test, but allows to be sure that nothing arrives to the second client
            client2ContentArrived.Should().BeNullOrEmpty();

            client1ContentArrived = string.Empty;
            client2ContentArrived = string.Empty;
            messageArrivedOnClient1.Reset();
            messageArrivedOnClient2.Reset();

            // the second message should arrive only to the second client
            var secondMessage = "The second message";
            client1.Publish(argument2, secondMessage);
            messageArrivedOnClient2.WaitOne(1000);
            client2ContentArrived.Should().Be(secondMessage);
            messageArrivedOnClient1.WaitOne(1000); // this will slow down the test, but allows to be sure that nothing arrives to the first client
            client1ContentArrived.Should().BeNullOrEmpty();

            client1ContentArrived = string.Empty;
            client2ContentArrived = string.Empty;
            messageArrivedOnClient1.Reset();
            messageArrivedOnClient2.Reset();

            // the third to both
            var commonMessage = "The common message";
            client1.Publish(commonArgument, commonMessage);
            messageArrivedOnClient1.WaitOne(1000);
            client1ContentArrived.Should().Be(commonMessage);
            messageArrivedOnClient2.WaitOne(1000);
            client2ContentArrived.Should().Be(commonMessage);
        }
    }
}
