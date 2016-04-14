using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace DistributedSystem
{
    class Node
    {
        public bool addNode(string IPPort)
        {
            Network network = Network.Instance;

            Console.WriteLine("One new node joined...");
            HostIPPort newNode = new HostIPPort(IPPort);
            int i = 0;

            while ( i < network.allNodes.Count && network.allNodes[i].hostIPComparison(newNode) < 0) 
                i++;

            network.allNodes.Insert(i, newNode);
            network.Online = true;

            return true;
        }

        public bool signoffNode(string IPPort)
        {
            Network network = Network.Instance;

            Console.WriteLine("One node signed off...");
            int i = 0;

            while (i < network.allNodes.Count() && !network.allNodes[i].getIPPort().Equals(IPPort)) 
                i++;

            if (i < network.allNodes.Count())
            {
                HostIPPort removedNode = network.allNodes[i];
                network.allNodes.Remove(removedNode);
            }

            // check master node or not
            string masterNodeUrl = network.masterNodeFullUrlID.Split('_').ElementAt(0);
            //Console.WriteLine("masterNodeUrl: " + masterNodeUrl);
            //Console.WriteLine("IPPort: " + IPPort);
            if (masterNodeUrl.Contains(IPPort))
            {
                Console.WriteLine("Master node signed off...");
                network.masterNodeFullUrlID = "";
            }

            //if (network.isSelfMasterNode == true)
            //{
            //    network.isSelfMasterNode = false;
            //    network.masterNodeFullUrlID = "";
            //}

            return true;
        }

        public string[] getAllNodes()
        {
            Network network = Network.Instance;

            Console.WriteLine("Return all nodes...");

            var result = new string[network.allNodes.Count];

            for (int i = 0; i < result.Length; i++) 
                result[i] = network.allNodes[i].getIPPort();

            Console.WriteLine("Get all nodes done...");
            return result;
        }

        public bool setNodeId(string[] nodesIPPortID)
        {
            Console.WriteLine("Set node ID...");

            Network network = Network.Instance;
            network.idAssigned = true;

            int count = 0;
            foreach (HostIPPort hIPP in network.allNodes)
            {
                int k = 0;
                string[] IPPortOrID = new string[2];
                do 
                {
                    IPPortOrID = nodesIPPortID[k].Split('_');
                    if (IPPortOrID[0].CompareTo(hIPP.getUrl())==0)
                    {
                        network.allNodes[count].setID(Int32.Parse(IPPortOrID[1]));
                        break;
                    }
                    k++;
                } while (k < nodesIPPortID.Count());

                if (IPPortOrID[0].CompareTo(network.SelfIPPort.getUrl())==0)
                {
                    network.SelfIPPort.setID(Int32.Parse(IPPortOrID[1]));
                    network.nodeID = Int32.Parse(IPPortOrID[1]);
                }
                //while (nodesIPPortID[k].hostIPComparison(hIPP))
                //{
                //    //string[] IPPortOrID = nodesIPPortID[i].Split('_');
                //}

                //if (hIPP.Equals(network.SelfIPPort))
                //{
                //    network.nodeID = hIPP.getID();
                //    network.SelfIPPort.setID(hIPP.getID());
                //}
                //else
                //{

                //}

                count++;
            } // foreach (HostIPPort hIPP in network.activeNodes)

            Console.WriteLine("\n+++Node List+++");
            foreach (HostIPPort hIPP in network.allNodes)
                Console.WriteLine(hIPP.getUrl() + "   ID:" + hIPP.getID().ToString());
            Console.WriteLine("---Node List---\n");
            
            return true;
        }

        public int bullyAlgorithm(string fullUrlID)
        {
            Random rand = new Random();
            int randomNum = rand.Next(2);   // 1 means to send OK back and want to take over
            //int randomNum = 1;

            if (randomNum == 1)
            {
                Thread bullyAlgorithmThread = new Thread(new ThreadStart(BullyAlgorithm.start));
                bullyAlgorithmThread.Start();
                //BullyAlgorithm.start();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool announceMaster(string masterFullUrlID)
        {
            Network network = Network.Instance;
            
            network.isSelfMasterNode = false;
            network.masterNodeFullUrlID = masterFullUrlID;

            Console.WriteLine("======= " + network.masterNodeFullUrlID + " is the master node. =======");

            return true;
        }

        //public bool start(string messageToBeSent)
        //{
        //    return true;
        //}

        public bool setAlgorithmAndStarted(int algorithm)
        {
            Network network = Network.Instance;
            Agrawala agrawala = Network.agrawala;

            network.mutualExclusionAlgorithm = algorithm;

            if (network.mutualExclusionAlgorithm == 1)
                Console.WriteLine("\nUse Centralised Mutual Exclusion Algorithm.");
            else if (network.mutualExclusionAlgorithm == 2)
            {
                Console.WriteLine("\nUse Ricart & Agrawala Mutual Exclusion Algorithm.");
                agrawala.lamportClock.reset();
            }

            network.Started = true;
            Console.WriteLine(network.SelfIPPort.getUrlID() + " STARTS!!!\n\n");

            return true;
        }
    }
}
