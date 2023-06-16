using PubSub.Client.TCPClient;
using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PubSub.Client
{
    /// <summary>
    /// This factory creates instances of a client
    /// </summary>
    public static class ChannelClientFactory
    {
        private static Dictionary<CommunicationType, Type> s_clientsTypes = new Dictionary<CommunicationType, Type>();

        static ChannelClientFactory()
        {
            Register(CommunicationType.BasicTCP, typeof(BasicTCPClient));
        }

        /// <summary>
        /// Register a type for a given comunicationType
        /// </summary>
        /// <param name="communicationType">Enum refered to a specific communication type</param>
        /// <param name="type">Type of the client</param>
        public static void Register(CommunicationType communicationType, Type type)
        {
            s_clientsTypes[communicationType] = type;
        }

        /// <summary>
        /// Creates a new instance of a IChannelClient. Standard type is a BasicTCP communication type
        /// </summary>
        /// <param name="communicationType">Optional communication type used by the client</param>
        /// <param name="configurationAction">Optional configuration action</param>
        /// <returns></returns>
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
