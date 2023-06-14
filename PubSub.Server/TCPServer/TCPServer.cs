using PubSub.Shared;
using PubSub.Shared.TCP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Server.TCPServer
{
    internal class TCPServer : IChannelServer
    {
        private bool _disposed;
        private Socket _tcpServer;
        private IPubSubLogger _logger;
        private TCPServerConfiguration _configuration;
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
            if (_disposed)
                throw new ObjectDisposedException(nameof(TCPServer));

            _logger?.Info($"{nameof(TCPServer)}: Initialization started...");

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _configuration.Port);
            _tcpServer = new Socket( endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _tcpServer.Bind(endPoint);

            Task.Run(ManageClient, _cancellationTokenSource.Token);
            _logger?.Info($"{nameof(TCPServer)}: Initialization finished...");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                _tcpServer?.Close();
                _cancellationTokenSource.Cancel();
                Thread.Sleep(100);

                _tcpServer?.Dispose();
                _tcpServer = null;
            }
            catch
            {
            }
        }

        private void ManageClient()
        {
            _tcpServer.Listen(100);

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var newClient = _tcpServer.Accept();

                Task.Run(() => HandleNewClient(newClient));
            }

        }

        private void HandleNewClient(Socket clientSocket)
        {
            var handle = clientSocket.Handle;
            _logger?.Info($"New Client[{handle}] connected");
            string plainMessage = string.Empty;
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var dataBuffer = new byte[1024];
                var numOfBytes = clientSocket.Receive(dataBuffer, SocketFlags.None);
                plainMessage += Encoding.UTF8.GetString(dataBuffer, 0, numOfBytes);

                if(numOfBytes <= 0)
                {
                    // The connection has been interruped by the client
                    _logger?.Info($"Client[{handle}] disconnected");
                    break;
                }

                var messageTerminator = "\r\n";
                if (plainMessage.IndexOf(messageTerminator) > -1)
                {
                    // The message is complete
                    if (plainMessage.IndexOf('\0') > -1)
                        plainMessage = plainMessage.Substring(0, plainMessage.IndexOf('\0'));

                    _logger?.Info($"Received message: {plainMessage} from Client[{handle}]");
                    var decodedMessage = TCPMessageParser.Decode(plainMessage);
                    if (decodedMessage != null)
                    {
                        if (!_channels.TryGetValue(decodedMessage.Channel, out var clients))
                        {
                            _channels.TryAdd(decodedMessage.Channel, new List<Socket>());
                        }

                        if (decodedMessage.MessageType == MessageType.Publish)
                        {
                            // I have to publish to the subscription list
                            clients.ForEach(x => SendContentMessage(decodedMessage, x));
                        }
                        else if (decodedMessage.MessageType == MessageType.Subscribe)
                        {
                            if (_channels.TryGetValue(decodedMessage.Channel, out var subscribers))
                            {
                                // I have to register it if it wasn't already registered
                                if(!subscribers.Contains(clientSocket))
                                    subscribers.Add(clientSocket);
                            }
                        }
                        SendAckMessage(clientSocket);
                    }
                    else
                    {
                        SendErrorMessage(clientSocket);
                    }
                    // reset the message info
                    plainMessage = string.Empty;
                }
            }
        }

        private void SendContentMessage(MessageInfo decodedMessage, Socket clientSocket)
        {
            var contentMessage = TCPMessageParser.CreateContentMessage(decodedMessage.Channel, decodedMessage.Message);
            if (contentMessage is null)
                return;

            clientSocket.Send(Encoding.UTF8.GetBytes(contentMessage));
        }

        private void SendAckMessage(Socket clientSocket)
        {
            var contentMessage = TCPMessageParser.CreateAckMessage();
            if (contentMessage is null)
                return;

            clientSocket.Send(Encoding.UTF8.GetBytes(contentMessage));
        }

        private void SendErrorMessage(Socket clientSocket)
        {
            var contentMessage = TCPMessageParser.CreateErrorMessage();
            if (contentMessage is null)
                return;

            clientSocket.Send(Encoding.UTF8.GetBytes(contentMessage));
        }

    }
}
