using PubSub.Shared;
using PubSub.Shared.TCP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Client.TCPClient
{
    internal class BasicTCPClient : IChannelClient
    {
        private bool _disposed;
        IPEndPoint _serverEndPoint;
        private IPubSubLogger _logger;
        private IMessageParser _parser;
        private BasicTCPClientConfiguration _configuration;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ConcurrentDictionary<string,List<IChannelSubscriber>> _subscriptions = new ConcurrentDictionary<string, List<IChannelSubscriber>>();

        public BasicTCPClient(Action<IClientConfigurationSettings> configuration = null)
        {
            _configuration = new BasicTCPClientConfiguration();
            configuration?.Invoke(_configuration);
            _logger = _configuration.Logger;
            _parser = _configuration.Parser;

            IPHostEntry ipHostInfo = Dns.GetHostEntry(_configuration.Host);
            IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            _serverEndPoint = new IPEndPoint(ipAddress, _configuration.Port);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _cancellationTokenSource.Cancel();

            _subscriptions.Clear();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public bool Publish(string channel, string message)
        {
            bool publishResult = true;
            try
            {
                
                using (var publishingClient = new Socket(_serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    publishingClient.Connect(_configuration.Host, _configuration.Port);
                    if (!publishingClient.Connected)
                        return false;

                    var encodedMessage = _parser.CreatePublishMessage(channel, message);
                    if (encodedMessage is null)
                        return false;

                    var sendingBytes = Encoding.UTF8.GetBytes(encodedMessage);
                    publishingClient.Send(sendingBytes);

                    var answer = WaitForNextMessage(publishingClient);
                    if (answer is null || answer.MessageType == MessageType.Error)
                    {
                        publishResult = false;
                    }

                    publishingClient.Close();
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Exception on publishing {message} on channel {channel}. Exception message: {ex}");
                publishResult = false;
            }

            return publishResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public bool Subscribe(string channel, IChannelSubscriber subscriber)
        {
            if (string.IsNullOrEmpty(channel))
                return false;
            if(subscriber is null)
                return false;

            if (_subscriptions.TryGetValue(channel.ToUpperInvariant(), out var actions))
            {
                actions.Add(subscriber);
                return true;
            }

            var subscription = new Socket(_serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            subscription.Connect(_configuration.Host, _configuration.Port);
            if (!subscription.Connected)
            {
                subscription.Dispose();
                return false;
            }

            string message = _parser.CreateSubscribeMessage(channel);
            if (message is null)
            {
                subscription.Close();
                subscription.Dispose();
                return false;
            }

            var sendingBytes = Encoding.UTF8.GetBytes(message);
            subscription.Send(sendingBytes);

            var answer = WaitForNextMessage(subscription);
            if(answer is null || answer.MessageType == MessageType.Error)
            {
                subscription.Close();
                subscription.Dispose();
                return false;
            }

            _subscriptions[channel] = new List<IChannelSubscriber> { subscriber };
            Task.Run(() => ManageSubscription(channel, subscription));

            return true;
        }

        private IMessageInfo WaitForNextMessage(Socket subscription)
        {
            var received = new byte[subscription.ReceiveBufferSize];
            subscription.Receive(received, SocketFlags.None);
            var receivedString = Encoding.UTF8.GetString(received);
            return _parser.Decode(receivedString);
        }

        private void ManageSubscription(string channel, Socket client)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var message = WaitForNextMessage(client); 
                if (message is null) 
                {
                    break;
                }
                if(message.MessageType == MessageType.Content)
                {
                    if(_subscriptions.TryGetValue(channel, out var subscribers))
                    {
                        Task.Run(() => subscribers.ForEach(subscriber => subscriber.OnMessage(message.Channel, message.Content)));
                    }
                }
            }

            client.Close();
            client.Dispose();
            _subscriptions.TryRemove(channel, out var closingSubscribers);
            closingSubscribers.ForEach(x => x.OnClose(channel));
        }
    }
}
