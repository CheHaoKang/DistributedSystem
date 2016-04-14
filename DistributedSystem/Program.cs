using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter port to start: ");

            int port = int.Parse(Console.ReadLine());
            InputReader iReader = new InputReader(port);
            
            Thread iReaderThread = new Thread(new ThreadStart(iReader.execute));
            iReaderThread.Start();           
        }
    }
}
