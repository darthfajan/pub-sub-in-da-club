using PubSub.Server.TCPServer;
using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Server
{
    /// <summary>
    /// This factory creates instances of a server
    /// </summary>
    public static class ChannelServerFactory
    {
        private static Dictionary<CommunicationType, Type> s_serverTypes = new Dictionary<CommunicationType, Type>();

        static ChannelServerFactory()
        {
            Register(CommunicationType.BasicTCP, typeof(BasicTCPServer));
        }

        /// <summary>
        /// Register a type for a given comunicationType
        /// </summary>
        /// <param name="communicationType">Enum refered to a specific communication type</param>
        /// <param name="type">Type of the server</param>
        public static void Register(CommunicationType communicationType, Type type)
        {
            s_serverTypes[communicationType] = type;
        }

        /// <summary>
        /// Creates a new instance of a IChannelServer. Standard type is a BasicTCP communication type
        /// </summary>
        /// <param name="communicationType">Optional communication type used by the server</param>
        /// <param name="configurationAction">Optional configuration action</param>
        /// <returns></returns>
        public static IChannelServer CreateServer(CommunicationType communicationType = CommunicationType.BasicTCP, Action<IChannelServerConfiguration> configurationAction = null)
        {
            if (!s_serverTypes.TryGetValue(communicationType, out var serverType))
            {
                return null;
            }
            try
            {
                return (IChannelServer)Activator.CreateInstance(serverType, new[] { configurationAction });
            }
            catch
            {
                return null;
            }
        }
    }
}
