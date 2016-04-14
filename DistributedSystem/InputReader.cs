using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedSystem
{
    class InputReader
    {
        private Client client;
        private Server server;
        private Thread clientThread;
        private Thread serverThread;

        public InputReader(int port)
        {
            client = new Client(port);
            server = new Server(port);

            serverThread = new Thread(new ThreadStart(server.execute));
            serverThread.Start();

            clientThread = new Thread(new ThreadStart(client.execute));
            clientThread.Start();
        }

        public void execute()
        {
            Console.WriteLine("\nWhich one you want to do: ");
            Console.WriteLine("Join     Signoff     Start\n");

            while (true)
            {
                String input = Console.ReadLine();

                if (input != null)
                {
                    Console.WriteLine("\nYou are going to " + input + "...");
                }

                int option = inputOptions(input);

                Network network;
                switch (option)
                {
                    case 0:
                        { 
                            Console.WriteLine("Joining...");
                            network = Network.Instance;
                            network.join();
                            break;
                        }
                    case 1:
                        { 
                            Console.WriteLine("Signing off...");
                            network = Network.Instance;
                            network.signOff();
                            break;
                        }
                    case 2:
                        { 
                            Console.WriteLine("Start sending messages...\n");
                            network = Network.Instance;
                            network.start();
                            break;
                        }

                    case 3:
                        {
                            Console.WriteLine("The input is wrong!!!");
                            break;
                        }
                }
            }

        }

        public int inputOptions(String input)
        {
            if (input.Equals("Join"))
            {
                return 0;
            }
            if (input.Equals("Signoff"))
            {
                return 1;
            }
            if (input.Equals("Start"))
            {
                return 2;
            }
            return 3;
        }
    }
}
