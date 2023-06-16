using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Shared
{
    /// <summary>
    /// The PubSub logger interface for debug purpose
    /// </summary>
    public interface IPubSubLogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}
