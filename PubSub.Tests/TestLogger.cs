using PubSub.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSub.Tests
{
    internal class TestLogger : IPubSubLogger
    {
        private readonly TestContext _context;

        public TestLogger(TestContext context)
        {
            _context = context;
        }

        public void Info(string message)
        {
            _context.WriteLine($"{message}");
        }

        public void Warn(string message)
        {
            _context.WriteLine($"WARN - {message}");
        }

        public void Error(string message)
        {
            _context.WriteLine($"ERROR - {message}");
        }
    }
}
