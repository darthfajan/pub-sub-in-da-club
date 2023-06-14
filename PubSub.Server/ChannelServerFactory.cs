using PubSub.Server.TCPServer;
using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server
{
    public static class ChannelServerFactory
    {
        private static Dictionary<CommunicationType, Type> s_serverTypes = new Dictionary<CommunicationType, Type>();

        static ChannelServerFactory()
        {
            Register(CommunicationType.BasicTCP, typeof(BasicTCPServer));
        }

        public static void Register(CommunicationType communicationType, Type type)
        {
            s_serverTypes[communicationType] = type;
        }

        public static IChannelServer CreateServer(CommunicationType communicationType = CommunicationType.BasicTCP, Action<IChannelServerConfiguration> configuration = null)
        {
            if (!s_serverTypes.TryGetValue(communicationType, out var serverType))
            {
                return null;
            }

            return (IChannelServer)Activator.CreateInstance(serverType, new[] { configuration });
        }
    }
}
