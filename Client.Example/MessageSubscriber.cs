using PubSub.Client;

namespace Client.Example
{
    internal class MessageSubscriber : IChannelSubscriber
    {
        public void OnClose(string channel)
        {
            Console.WriteLine($"CLOSING: {channel}");
        }

        public void OnMessage(string channel, string message)
        {
            Console.WriteLine($"MESSAGE: {channel} -- {message}");
        }
    }
}