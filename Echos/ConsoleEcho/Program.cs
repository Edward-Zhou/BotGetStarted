using System;

namespace ConsoleEcho
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the EchoBot. Type something to get started.");
            var adapter = new ConsoleAdapter();

            var echoBot = new EchoBot();

            adapter.ProcessActivityAsync(
                async (turnContext, cancellationToken) => await echoBot.OnTurnAsync(turnContext)).Wait();
        }
    }
}
