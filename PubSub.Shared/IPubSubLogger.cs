using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Shared
{
    public interface IPubSubLogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}
