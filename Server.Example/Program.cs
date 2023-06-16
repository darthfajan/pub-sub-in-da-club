using PubSub.Server;

namespace Server.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WELCOME TO PUB/SUB IN DA CLUB SERVER EXAMPLE!");
            Console.WriteLine(@"This simple console application demonstrate the pubblication\subscription features of a server.");
            Console.WriteLine(@"The server is already running.");
            Console.WriteLine(@"");
            Console.WriteLine(@"You can execute the following actions:");
            Console.WriteLine(@"exit : closes the connection and the program");

            using var server = ChannelServerFactory.CreateServer();
            server.Init();
            bool exit = false;
            while (!exit)
            {
                var nextCommand = Console.ReadLine();

                if (string.IsNullOrEmpty(nextCommand))
                {
                    continue;
                }

                if (nextCommand.ToString().ToUpperInvariant() == "EXIT")
                {
                    exit = true;
                    continue;
                }

            }
            Console.WriteLine(@"Bye bye");
        }
    }
}