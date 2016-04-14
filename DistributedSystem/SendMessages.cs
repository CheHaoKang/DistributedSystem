using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using CookComputing.XmlRpc;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedSystem
{
    class SendMessages
    {
        /*
         * request = 1
         *  Nodes want to read the string variable
         * request = 2
         *  Nodes want to append words to the string variable
         */
        public string[] requestToMaster(string nodeFullUrlID)
        {
            Console.WriteLine("***Receive the request from " + nodeFullUrlID + "***");

            Master master = Master.Instance;
            object mutualLock = Master.mutualLock;
            Network network = Network.Instance;
            string[] returnString = new string[2];

            try
            {
                lock (mutualLock)
                {
                    if (master.occupied != "")  // != "" means the master is busy
                    {
                        if (!master.nodeQueue.Contains(nodeFullUrlID))
                        {
                            master.nodeQueue.Enqueue(nodeFullUrlID);
                            Console.WriteLine("***Put " + nodeFullUrlID + " into the queue...***");
                            //Console.WriteLine("______+++_____");
                            //Console.WriteLine("Enqueue: " + nodeUrlID);
                        }

                        //Console.WriteLine("the master node is busy From requestToMaster...nodeUrlID: " + nodeUrlID + " waiting...");
                        Queue<string> tmpQueue = new Queue<string>(master.nodeQueue);
                        //Console.WriteLine("inside lock(mutualLock)");
                        //foreach (string queue in tmpQueue)
                        //{
                        //    Console.WriteLine("print queue: " + queue.ToString());
                        //}
                        //Console.WriteLine("______---_____");
                        
                        returnString[0] = "busy";
                        returnString[1] = "";
                        return returnString;
                    }
                    else
                    {
                        //Console.WriteLine("Outside if (announceClientMasterIsFree())");
                        if (announceClientMasterIsFree(nodeFullUrlID))
                        {
                            //master.occupied = nodeUrlID;
                            Console.WriteLine("***The master node is free for " + nodeFullUrlID + "***");
                            returnString[0] = "free";
                            returnString[1] = master.masterString;
                            return returnString;
                        }
                        else
                        {
                            //Console.WriteLine("before return busy");
                            returnString[0] = "busy";
                            returnString[1] = "";
                            return returnString;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("In requestToMaster - Exception source: {0}  {1}", e.Source, e.ToString());
                throw;
            }
        }

        public string appendStringToMaster(string nodeUrlID, string addedString)
        {
            Master master = Master.Instance;
            object mutualLock = Master.mutualLock;
            Network network = Network.Instance;

            Console.WriteLine("***" + nodeUrlID + " is appending...***");

            lock (mutualLock)
            {
                master.masterString = addedString;

                if (network.mutualExclusionAlgorithm == 1) // 1 means using Centralized (only Centralized needs this)
                    announceClientMasterIsFree("");

                return master.masterString;
            }
        }

        public bool announceClientMasterIsFree(string nodeFullUrlID)
        {
            Master master = Master.Instance;
            object mutualLock = Master.mutualLock;
            string requestor;

            //Console.WriteLine("in announceClientMasterIsFree");
            try
            {
                lock(mutualLock)
                {
                    if (master.nodeQueue.Count != 0)
                    {
                        master.occupied = master.nodeQueue.Dequeue();
                        requestor = master.occupied;
                        Console.WriteLine("***Dequeue " + requestor + "***");
                        //master.nodeQueue.RemoveAt(0);
                        //requestor = master.nodeQueue.Dequeue();
                        //master.occupied = requestor;

                        //Console.WriteLine("if (master.nodeQueue.Count != 0)");
                        ISendMessages proxy = XmlRpcProxyGen.Create<ISendMessages>();

                        proxy.Url = requestor.Split('_').ElementAt(0);
                        //Console.WriteLine("in announce inside lock");
                        proxy.setWaitFlag(false, master.masterString);

                        return false;
                    }
                    else
                    {
                        //Console.WriteLine("in announce before master.occupied");
                        master.occupied = nodeFullUrlID;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("In announceClientMasterIsFree - Exception source: {0}  {1}", e.Source, e.ToString());
                throw;
            }
        }

        public bool setWaitFlag(bool waitFlag, string masterString)
        {
            Network network = Network.Instance;
            object mutualLock = network.mutualLock;

            //Console.WriteLine("setWaitFlag:" + waitFlag + " nodeID:" + network.SelfIPPort.getUrlID());
            //Console.WriteLine("Original masterString: " + masterString);
            lock (mutualLock)
            {
                network.masterString = masterString;
                //Console.WriteLine("before _network.wait = waitFlag_ - " + DateTime.Now.Ticks);
                network.wait = waitFlag;
            }
            //Console.WriteLine("network.wait:" + network.wait);

            return true;
        }

        // if end == true, we need to wait for all the nodes to finsih and then return the master string back
        public string getMasterString()
        {
        //    //Network network = Network.Instance;
        //    //object mutualLock = Master.mutualLock;

        //    // 1 means using Centralized (only Centralized needs this to get the master string in the end)
        //    //while ((master.finishedNodes != network.activeNodes.Count) && (end == true))
        //    //    ;

        //    // for Ricart & Agrawala, just return the master string.
            Master master = Master.Instance;
            return master.masterString;
        }

        public void notifyFinished(string node)
        {
            Network network = Network.Instance;
            Master master = Master.Instance;
            object mutualLock = Master.mutualLock;

            //Console.WriteLine("===notifyFinished==" + node);
            //Console.WriteLine("network.allNodes.Count: " + network.allNodes.Count);
            lock (mutualLock)
            {
                master.finishedNodes++;
                if (master.finishedNodes == network.allNodes.Count)
                {
                    foreach (HostIPPort hIPP in network.allNodes)
                    {
                        ISendMessages proxy = XmlRpcProxyGen.Create<ISendMessages>();
                        proxy.Url = hIPP.getUrl();
                        proxy.setWaitFlag(false, master.masterString);
                    }

                    master.masterString = "";
                    master.finishedNodes = 0;
                }
            }
        }
    }
}
