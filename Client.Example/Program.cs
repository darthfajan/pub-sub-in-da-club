using PubSub.Client;

namespace Client.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WELCOME TO PUB/SUB IN DA CLUB CLIENT EXAMPLE!");
            Console.WriteLine(@"This simple console application demonstrate the pubblication\subscription features of a client.");
            Console.WriteLine(@"A running instance of the server is required to run the demonstration.");
            Console.WriteLine(@"");
            Console.WriteLine(@"You can execute the following actions:");
            Console.WriteLine(@"pub <channel> <message> : publishes a message to a certain channel (e.g. pub drinks I need a beer)");
            Console.WriteLine(@"sub <channel> : subscribes to a channel and it will show the messages when they arrive (e.g. sub drinks)");
            Console.WriteLine(@"exit : closes the connection and the program");

            using var client = ChannelClientFactory.CreateClient();
            var sub = new MessageSubscriber();
            bool exit = false;
            while(!exit)
            {
                var nextCommand = Console.ReadLine();

                if (string.IsNullOrEmpty(nextCommand))
                {
                    continue;
                }

                if (nextCommand.ToString().ToUpperInvariant() == "EXIT") {
                    exit = true;
                    continue;
                }

                var instructions = nextCommand.Split(' ');
                if(instructions.Length == 2 && instructions[0].ToString().ToUpperInvariant() == "SUB")
                {
                    client.Subscribe(instructions[1], sub);
                    continue;
                }
                else if (instructions.Length > 2 && instructions[0].ToString().ToUpperInvariant() == "PUB")
                {
                    var message = nextCommand.Substring(instructions[1].Length + 5); // |pub channel |messaeg
                    client.Publish(instructions[1], message);
                    continue;
                }
                else
                {
                    Console.WriteLine("Invalid command");
                }
            }
            Console.WriteLine(@"Bye bye");
        }
    }
}