using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using CookComputing.XmlRpc;
using System.Runtime.Serialization;
using System.Timers;

namespace DistributedSystem
{
    public class Client
    {
        private readonly int _port;

        public Client(int port)
        {
            _port = port;
            startUp();
        }

        public void stopClient()
        {
            Network network = Network.Instance;
            network.Running = false;
        }

        private void startUp()
        {
            string IPAddress = null;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress IP in host.AddressList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork) //IPv4
                {
                    IPAddress = IP.ToString();
                    break;
                }
            }

            Network network = Network.Instance;
            network.SelfIPPort = new HostIPPort(IPAddress, _port);

            //Console.WriteLine("IP Adress: " + network.SelfIPPort.getIPPort());

            network.allNodes.Add(network.SelfIPPort);
            Console.WriteLine("IP address: " + network.SelfIPPort.getIPPort());
        }

        public void execute()
        {
            Console.WriteLine("Client is ON...");
            Network network = Network.Instance;
            object mutualLock = network.mutualLock;
            long startTime = 0;
            long elapsedTime = 0;
            int appendCounter = 0;

            while (network.Running)
            {
                if (network.Online)
                {
                    if (network.Started)
                    {

                        if (elapsedTime == 0)
                        {
                            startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        }

                        Console.WriteLine("\nIn the loop of sending messages...");

                        int max = 3000;
                        int min = 0;
                        Random rand = new Random();
                        int randomNum = rand.Next((max - min) + 1) + min;
                        Thread.Sleep(randomNum);

                        // Centralised Mutual Exclusion
                        if (network.mutualExclusionAlgorithm == 1)
                        {
                            ISendMessages proxy = XmlRpcProxyGen.Create<ISendMessages>();
                            while (network.masterNodeFullUrlID == "")
                                ;//Console.WriteLine("LOOP masterNodeFullUrlID:" + network.masterNodeFullUrlID);
                            proxy.Url = network.masterNodeFullUrlID.Split('_').ElementAt(0);
                            //Console.WriteLine("Master Node: " + network.masterNodeFullUrlID.Split('_').ElementAt(0));
                            //Console.WriteLine("before requestToMaster");
                            network.wait = true; // test
                            string[] repliedString = proxy.requestToMaster(network.SelfIPPort.getUrlID()); // inquire the master to get the handler
                            if (repliedString[0].Equals("busy"))
                            {
                                /*lock (mutualLock)
                                {
                                    long clientWaitFlagTime = DateTime.Now.Ticks;
                                    if ((clientWaitFlagTime - network.flagWaitSetTrueTime) <= 200000)
                                    //if (network.flagWaitSetTrueTime != 0)
                                    {
                                        network.wait = false;
                                        network.flagWaitSetTrueTime = 0;
                                    }
                                    else
                                        network.wait = true;
                                    Console.WriteLine("after _network.wait = true_ - " + clientWaitFlagTime);
                                }*/
                                //Console.WriteLine("The master node is busy...nodeID: " + network.SelfIPPort.getUrlID());
                                Console.WriteLine("======= The master node is busy...WAITING =======");
                                //Console.WriteLine("while(network.wait)");
                                while (network.wait)
                                {
                                    ;
                                }
                                Console.WriteLine("======= The master node is FREE...START to append =======");
                            }
                            else if (repliedString[0].Equals("free"))
                            {
                                network.masterString = repliedString[1];
                                //network.masterString = proxy.getMasterString(false);
                            }

                            //Console.WriteLine("Previous____masterString: " + network.masterString);
                            //string appendedRepliedString = proxy.appendStringToMaster(network.SelfIPPort.getUrlID(), network.masterString + "~~~DECKEN_" + network.SelfIPPort.getID() + "-" + (++appendCounter) + "~~~");
                            string appendedRepliedString = proxy.appendStringToMaster(network.SelfIPPort.getUrlID(), network.masterString + "Node" + network.SelfIPPort.getID() + "-" + (++appendCounter));
                            //Console.WriteLine("repliedString: " + repliedString);
                            Console.WriteLine("The appended string: " + "Node" + network.SelfIPPort.getID() + "-" + appendCounter + "\n");
                            //Console.WriteLine("after requestToMaster");
                        }
                        else if (network.mutualExclusionAlgorithm == 2) // Ricart & Agrawala
                        {
                            Console.WriteLine("Sending a request to use the resource...");
                            Agrawala agrawala = Network.agrawala;
                            if (agrawala.sendRequest())
                            {
                                ISendMessages proxy = XmlRpcProxyGen.Create<ISendMessages>();
                                while (network.masterNodeFullUrlID == "")
                                    ;// Console.WriteLine("LOOP masterNodeFullUrlID:" + network.masterNodeFullUrlID);
                                proxy.Url = network.masterNodeFullUrlID.Split('_').ElementAt(0);
                                network.masterString = proxy.getMasterString();
                                //string appendedRepliedString = proxy.appendStringToMaster(network.SelfIPPort.getUrlID(), network.masterString + "~~~DECKEN_" + network.SelfIPPort.getID() + "-" + (++appendCounter) + "~~~");
                                string appendedRepliedString = proxy.appendStringToMaster(network.SelfIPPort.getUrlID(), network.masterString + "Node" + network.SelfIPPort.getID() + "-" + (++appendCounter));
                            }

                            agrawala.releaseResource();
                            Console.WriteLine("Leave after sending messages...");
                            //instance.getAgrawala().releaseCS(); // release critical section after performing calculations
                            
                            //instance.getAgrawala().requestCS();
                            
                            //agrawa = new Agrawala(network.SelfIPPort.getUrl(), network.activeNodes.Count);
                           /* Agrawala agrawa = Agrawala.Instance;
                            Console.WriteLine("111");
                            agrawa.NodeId = network.SelfIPPort.getUrl();
                            Console.WriteLine("222");
                            agrawa.NodesNumber = network.activeNodes.Count;
                            Console.WriteLine("333");
                            //network.wait = true;

                            ISendMessages proxy = XmlRpcProxyGen.Create<ISendMessages>();
                            while (network.masterNodeFullUrlID == "")
                                Console.WriteLine("LOOP masterNodeFullUrlID:" + network.masterNodeFullUrlID);
                            proxy.Url = network.masterNodeFullUrlID.Split('_').ElementAt(0);

                            agrawa.sendRequest(network.activeNodes);
                            Console.WriteLine("DEFAULT !agrawa.canUseResource()");
                            while (!agrawa.canUseResource())
                            {
                                ;
                            }
                            Console.WriteLine("PASS !agrawa.canUseResource()");

                            network.masterString = proxy.getMasterString(false);
                            string appendedRepliedString = proxy.appendStringToMaster(network.SelfIPPort.getUrlID(), network.masterString + "~~~DECKEN_" + network.SelfIPPort.getID() + "-" + (++appendCounter) + "~~~");

                            string nextNodeUrl = agrawa.releaseResource();
                            if (nextNodeUrl != "")
                            {
                                IServerAgrawala agrawalaProxy = XmlRpcProxyGen.Create<IServerAgrawala>();
                                agrawalaProxy.Url = nextNodeUrl;
                                //INodeContractA nextNode = (INodeContractA)connectedNodes[nextNodeUrl];
                                agrawalaProxy.getOk(agrawa.Timestamp);
                            }*/
                        }

                        elapsedTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime;
                        if (elapsedTime >= 20000)
                        {
                            Console.WriteLine("Finish starting...");

                            network.wait = true;
                            ISendMessages proxy = XmlRpcProxyGen.Create<ISendMessages>();
                            while (network.masterNodeFullUrlID == "")
                                ;// Console.WriteLine("LOOP masterNodeFullUrlID:" + network.masterNodeFullUrlID);
                            proxy.Url = network.masterNodeFullUrlID.Split('_').ElementAt(0);
                            proxy.notifyFinished(network.SelfIPPort.getUrlID());
                            Console.WriteLine("\n======= Wait for other nodes to finish =======");
                            while (network.wait)
                            {
                                ;
                            }
                            //Console.WriteLine("Final Master String: " + network.masterString);
                            //Console.WriteLine("AppendCounter: " + appendCounter);

                            checkStringAppended(network.masterString, appendCounter);

                            network.Started = false;
                            startTime = 0;
                            elapsedTime = 0;
                            appendCounter = 0;
                        }
                    }
                }
            }
        }

        void checkStringAppended(String masterString, int appendCounter)
        {
            Network network = Network.Instance;

            Console.WriteLine("\n\n+++++++++Checking the master string starts+++++++++");

            bool pass = true;
            Console.WriteLine("Final Master String: \n" + network.masterString + "\n");
            Console.WriteLine("Number of Appending: " + appendCounter);
            Console.WriteLine("Node id: " + network.SelfIPPort.getID() + "\n");
            for (int i = 1; i <= appendCounter; i++)
            {
                if (!masterString.Contains("Node" + network.SelfIPPort.getID() + "-" + i.ToString()))
                {
                    Console.WriteLine("Node" + network.SelfIPPort.getID() + "-" + i.ToString() + " is MISSING...");
                    pass = false;
                }
                else
                {
                    Console.WriteLine("Node" + network.SelfIPPort.getID() + "-" + i.ToString() + " PASSES...");
                }
            }

            if (pass)
                Console.WriteLine("\nString checking PASSED!!!");
            else
                Console.WriteLine("\nString checking FAILED!!!");
            Console.WriteLine("---------Checking the master string ends---------\n\n");
        }

        public static void joinByXMLRPC(String fullURL)
        {
            Network network = Network.Instance;
            INode proxy = XmlRpcProxyGen.Create<INode>();
            proxy.Url = fullURL;
            proxy.addNode(network.SelfIPPort.getIPPort());
        }

        public static void signOffByXMLRPC(String fullURL)
        {
            Network network = Network.Instance;
            INode proxy = XmlRpcProxyGen.Create<INode>();
            proxy.Url = fullURL;
            proxy.signoffNode(network.SelfIPPort.getIPPort());
        }

        public static void assignIDByXMLRPC(String fullURL, string[] nodesIPPortID)
        {
            Network network = Network.Instance;
            INode proxy = XmlRpcProxyGen.Create<INode>();
            proxy.Url = fullURL;
            //Console.WriteLine("before XMLRPC");
            proxy.setNodeId(nodesIPPortID);
        }

        public static int bullyAlgorithmByXMLRPC(String fullURL)
        {
            Network network = Network.Instance;
            INode proxy = XmlRpcProxyGen.Create<INode>();
            proxy.Url = fullURL;
            return proxy.bullyAlgorithm(network.SelfIPPort.getUrlID());
        }

        public static void announceMasterByXMLRPC(String fullURL, string masterFullUrlID)
        {
            Network network = Network.Instance;
            INode proxy = XmlRpcProxyGen.Create<INode>();
            proxy.Url = fullURL;
            proxy.announceMaster(masterFullUrlID);
        }

        public static void setAlgorithmAndStartedByXMLRPC(string fullURL, int algorithm)
        {
            INode proxy = XmlRpcProxyGen.Create<INode>();
            proxy.Url = fullURL;
            proxy.setAlgorithmAndStarted(algorithm);
        }
    }
}
