using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Shared.TCP
{
    public class MessageInfo
    {
        public string Message { get; set; }
        public string Channel { get; set; }
        public MessageType MessageType { get; set; }
    }
}
