using PubSub.Shared;
using PubSub.Shared.TCP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Server.TCPServer
{
    internal class TCPServer : IChannelServer
    {
        private bool disposed;
        private Socket _tcpServer;
        private TCPServerConfiguration _configuration;
        private IPubSubLogger _logger;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        internal ConcurrentDictionary<string, List<Socket>> _channels = new ConcurrentDictionary<string, List<Socket>>();

        public TCPServer(Action<IChannelServerConfiguration> configuration = null)
        {
            _configuration = new TCPServerConfiguration();
            configuration?.Invoke(_configuration);
            _logger = _configuration.Logger;
        }

        public void Init()
        {
            _logger?.Info($"{nameof(TCPServer)}: Initialization started...");
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _configuration.Port);
            _tcpServer = new Socket(
                localEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            _tcpServer.Bind(localEndPoint);

            Task.Run(ManageClientsAsync, _cancellationTokenSource.Token);
            _logger?.Info($"{nameof(TCPServer)}: Initialization finished...");
        }

        private async Task ManageClientsAsync()
        {
            _tcpServer.Listen(100);


            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var handler = _tcpServer.Accept();

                Task.Run(async () => await HandleNewConnectionAsync(handler));
            }

        }

        private async Task HandleNewConnectionAsync(Socket handler)
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = handler.Receive(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var eom = "\r\n";
                if (response.IndexOf(eom) > -1 /* is end of message */)
                {
                    Console.WriteLine(
                        $"Socket server received message: \"{response.Replace(eom, "")}\"");

                    var decodedMessage = TCPMessageParser.Decode(response);
                    if (decodedMessage != null)
                    {
                        if (!_channels.TryGetValue(decodedMessage.Channel, out var clients))
                        {
                            _channels.TryAdd(decodedMessage.Channel, new List<Socket>());
                        }

                        if (decodedMessage.MessageType == MessageType.Publish)
                        {
                            clients.ForEach(x => SendMessage(decodedMessage, x));
                        }
                        else if (decodedMessage.MessageType == MessageType.Subscribe)
                        {
                            if (_channels.TryGetValue(decodedMessage.Channel, out var subscribers))
                            {
                                subscribers.Add(handler);
                            }
                        }
                    }
                }
                // Sample output:
                //    Socket server received message: "Hi friends 👋!"
                //    Socket server sent acknowledgment: "<|ACK|>"
            }
        }

        private void SendMessage(MessageInfo decodedMessage, Socket x)
        {
            var contentMessage = TCPMessageParser.CreateContentMessage(decodedMessage.Channel, decodedMessage.Message);
            x.Send(Encoding.UTF8.GetBytes(contentMessage));
        }

        public void Dispose()
        {
            if (disposed)
                return;
            try
            {
                _tcpServer?.Close();
                _tcpServer?.Dispose();
                _tcpServer = null;
            }
            catch
            {
            }
        }
    }
}
