using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSystem
{
    class BullyAlgorithm
    {
        public static void start()
        {
            Network network = Network.Instance;

            Console.WriteLine("+++ Bully Algorithm +++");
            foreach (HostIPPort hIPP in network.allNodes)
            {
                if (hIPP.getID() > network.SelfIPPort.getID())
                {
                    int returnValue = Client.bullyAlgorithmByXMLRPC(hIPP.getUrl());
                    Console.WriteLine(hIPP.getUrlID() + "   " + returnValue);

                    //if (Client.bullyAlgorithmByXMLRPC(hIPP.getUrl()) == 1)
                    if (returnValue == 1)
                    {
                        Console.WriteLine(hIPP.getUrlID() + " wants to take over...");
                        network.isSelfMasterNode = false;
                        Console.WriteLine("--- Bully Algorithm ---");
                        return;
                    }
                }
            }

            Console.WriteLine("=======" + network.SelfIPPort.getUrlID() + " is the master node... =======");
            network.masterNodeFullUrlID = network.SelfIPPort.getUrlID();
            network.isSelfMasterNode = true;

            Console.WriteLine("--- Bully Algorithm ---");

            foreach (HostIPPort hIPP in network.allNodes)
            {
                if (!hIPP.Equals(network.SelfIPPort))
                    Client.announceMasterByXMLRPC(hIPP.getUrl(), network.masterNodeFullUrlID);
            }
        }
    }
}
