using PubSub.Client.TCPClient;
using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PubSub.Client
{
    public static class ChannelClientFactory
    {
        private static Dictionary<CommunicationType, Type> s_clientsTypes = new Dictionary<CommunicationType, Type>();

        static ChannelClientFactory()
        {
            Register(CommunicationType.BasicTCP, typeof(BasicTCPClient));
        }

        public static void Register(CommunicationType communicationType, Type type)
        {
            s_clientsTypes[communicationType] = type;
        }

        public static IChannelClient CreateClient(CommunicationType communicationType = CommunicationType.BasicTCP, Action<IClientConfigurationSettings> configurationAction = null)
        {
            if (!s_clientsTypes.TryGetValue(communicationType, out var clientType))
            {
                return null;
            }

            try
            {
                return (IChannelClient)Activator.CreateInstance(clientType, new[] { configurationAction });

            }
            catch
            {
                return null;
            }
        }
    }
}
