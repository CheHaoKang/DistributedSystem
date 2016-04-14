using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DistributedSystem
{
    public sealed class Network
    {
        private static readonly Network _network = new Network();
        private static readonly object _mutualLock = new object();
        private static bool _online = false;
        public static List<HostIPPort> _allNodes = new List<HostIPPort>();
        private static HostIPPort _selfIPPort;
        private static string _masterNodeFullUrlID = "";
        private HostIPPort _connectToNode;
        private int _nodeID;
        private bool _running = true;
        private bool _started = false;

        //+++ Centralized
        private bool _idAssigned = false;
        private bool _isSelfMasterNode = false;
        private int _mutualExclusionAlgorithm = 0; // 1 means using Centralized; 2 means using Ricart & Agrawala
        private bool _wait = false;
        private static string _masterString = "";
        //--- Centralized

        //+++ Agrawala
        private static Agrawala _agrawala = new Agrawala();
        //private LamportClock _LamportClock = new LamportClock();
        //private int _replicateClock = Int32.MaxValue;
        //--- Agrawala

        private Network()
        {
        }

        public static Network Instance
        {
            get
            {
                return _network;
            }
        }

        public bool Running
        {
            get
            {
                lock (_mutualLock)
                {
                    return _running;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _running = value;
                }
            }
        }

        public bool Online
        {
            get
            {
                lock (_mutualLock)
                {
                    return _online;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _online = value;
                }
            }
        }

        public bool Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public HostIPPort SelfIPPort
        {
            get
            {
                lock (_mutualLock)
                {
                    return _selfIPPort;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _selfIPPort = value;
                }
            }
        }

        public List<HostIPPort> allNodes
        {
            get { return _allNodes; }
            set { _allNodes = value; }
        }

        public HostIPPort connectToNode
        {
            get
            {
                lock (_mutualLock)
                {
                    return _connectToNode;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _connectToNode = value;
                }
            }
        }

        public bool idAssigned
        {
            get
            {
                lock (_mutualLock)
                {
                    return _idAssigned;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _idAssigned = value;
                }
            }
        }

        public string masterNodeFullUrlID
        {
            get
            {
                lock (_mutualLock)
                {
                    return _masterNodeFullUrlID;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _masterNodeFullUrlID = value;
                }
            }
        }

        public bool isSelfMasterNode
        {
            get
            {
                lock (_mutualLock)
                {
                    return _isSelfMasterNode;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _isSelfMasterNode = value;
                }
            }
        }

        public int nodeID
        {
            get
            {
                lock (_mutualLock)
                {
                    return _nodeID;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _nodeID = value;
                }
            }
        }

        public int mutualExclusionAlgorithm
        {
            get
            {
                lock (_mutualLock)
                {
                    return _mutualExclusionAlgorithm;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _mutualExclusionAlgorithm = value;
                }
            }
        }

        public bool wait
        {
            get
            {
                lock (_mutualLock)
                {
                    return _wait;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _wait = value;
                }
            }
        }

        public string masterString
        {
            get
            {
                lock (_mutualLock)
                {
                    return _masterString;
                }
            }
            set
            {
                lock (_mutualLock)
                {
                    _masterString = value;
                }
            }
        }

        public object mutualLock
        {
            get
            {
                return _mutualLock;
            }
        }

        public static Agrawala agrawala
        {
            get
            {
                return _agrawala;
            }
            set
            {
                lock (_mutualLock)
                {
                    _agrawala = value;
                }
            }
        }

        //public int replicateLock
        //{
        //    get
        //    {
        //        return _replicateClock;
        //    }
        //    set
        //    {
        //        lock (_mutualLock)
        //        {
        //            _replicateClock = value;
        //        }
        //    }
        //}

        //public long flagWaitSetTrueTime
        //{
        //    get
        //    {
        //        lock (_mutualLock)
        //        {
        //            return _flagWaitSetTrueTime;
        //        }
        //    }
        //    set
        //    {
        //        lock (_mutualLock)
        //        {
        //            _flagWaitSetTrueTime = value;
        //        }
        //    }
        //}

        public int join()
        {
            lock (_mutualLock)
            {
                Network network = Network.Instance;

                if (!network.Online)
                {
                    Console.WriteLine("IP address to connect to: ");
                    String IP = Console.ReadLine();

                    Console.WriteLine("Port of the IP address: ");
                    int port = int.Parse(Console.ReadLine());

                    network.connectToNode = new HostIPPort(IP, port); // the node we want to connect to

                    INode proxy = XmlRpcProxyGen.Create<INode>();
                    proxy.Url = network.connectToNode.getUrl();  // Server.cs - "xmlrpc"

                    Object[] NodesFromRPC = proxy.getAllNodes();

                    for (int i = 0; i < NodesFromRPC.Length; i++)
                    {
                        HostIPPort newNode = new HostIPPort(NodesFromRPC[i].ToString());
                        int k = 0;
                        while (k < network.allNodes.Count && network.allNodes[k].hostIPComparison(newNode) < 0) 
                            k++;
                        network.allNodes.Insert(k, newNode);
                    }

                    network.Online = true;

                    foreach (HostIPPort hIPP in network.allNodes)
                    {
                        if (!hIPP.Equals(network.SelfIPPort))
                            Client.joinByXMLRPC(hIPP.getUrl());
                    }

                    return 1;
                }
                else
                {
                    Console.WriteLine("Node exists!");
                    return 0;
                }
            }
        }

        public int signOff()
        {
            lock (_mutualLock)
            {
                Network network = Network.Instance;

                if (network.Online)
                {
                    foreach (HostIPPort hIPP in network.allNodes)
                    {
                        if (!hIPP.Equals(network.SelfIPPort))
                            Client.signOffByXMLRPC(hIPP.getUrl());
                    }

                    network.Online = false;
                    network.allNodes.Clear();
                    network.allNodes.Add(network.SelfIPPort);

                    // check master node or not
                    if (network.isSelfMasterNode == true)
                    {
                        network.isSelfMasterNode = false;
                        network.masterNodeFullUrlID = "";
                    }

                    return 1;
                }
                else
                {
                    Console.WriteLine("Already signed off...");
                    return 0;
                }
            }
        }

        public int start()
        {
            lock (_mutualLock)
            {
                Network network = Network.Instance;

                if (network.Online)
                {
                    if (!network.Started)
                    {
                        if (!network.idAssigned)
                        {
                            bool[] numberAssigned = new bool[network.allNodes.Count];
                            for (int i = 0; i < network.allNodes.Count; i++)
                            {
                                numberAssigned[i] = false;
                            }

                            int count = 0;
                            //Random rand = new Random();
                            //int randomNum;
                            string[] nodesIPPortID = new string[network.allNodes.Count];

                            // test +++
                            ////if (network.activeNodes[count].Equals(network.SelfIPPort))
                            ////{
                            //    _idAssigned = true;
                            //    _nodeID = 0;
                            //    network.SelfIPPort.setID(0);
                            ////}

                            //numberAssigned[0] = true;
                            ////count++;

                            //while (count < network.activeNodes.Count)
                            //{
                            //    if (network.activeNodes[count].Equals(network.SelfIPPort))
                            //    {
                            //        network.activeNodes[count].setID(0);
                            //        nodesIPPortID[count] = network.activeNodes[count].getUrl() + "_0";
                            //    }
                            //}

                            //count = 1;
                            // test ---

                            /* test +++
                            while (count < network.activeNodes.Count)
                            {
                                while (true)
                                {
                                    randomNum = rand.Next(network.activeNodes.Count);
                                    //randomNum = rand.Next(1, network.activeNodes.Count); 
                                    if (!numberAssigned[randomNum])
                                    {
                                        //HostIPPort hIPP = network.activeNodes[count];
                                        //Client.assignIDByXMLRPC(hIPP.getUrl(), randomNum);
                                        // test +++
                                        if (network.activeNodes[count].Equals(network.SelfIPPort))
                                        {
                                            _idAssigned = true;
                                            _nodeID = randomNum;
                                            network.SelfIPPort.setID(randomNum);
                                        }
                                        //test ---

                                        network.activeNodes[count].setID(randomNum);
                                        nodesIPPortID[count] = network.activeNodes[count].getUrl() + "_" + randomNum.ToString();

                                        numberAssigned[randomNum] = true;
                                        count++;

                                        break;
                                    }
                                }
                            } test ---*/

                            // test +++
                            int num = 1;
                            while (count < network.allNodes.Count)
                            {
                                if (network.allNodes[count].Equals(network.SelfIPPort))
                                {
                                    _idAssigned = true;
                                    _nodeID = 0;
                                    network.SelfIPPort.setID(0);
                                    network.allNodes[count].setID(0);
                                    nodesIPPortID[count] = network.allNodes[count].getUrl() + "_0";
                                }
                                else
                                {
                                    network.allNodes[count].setID(num);
                                    nodesIPPortID[count] = network.allNodes[count].getUrl() + "_" + num.ToString();
                                    num++;
                                }
                                
                                count++;
                            }
                            // test ---

                            foreach (HostIPPort hIPP in network.allNodes)
                            {
                                if (!hIPP.Equals(network.SelfIPPort))
                                {
                                    Client.assignIDByXMLRPC(hIPP.getUrl(), nodesIPPortID);
                                }
                            }

                            Console.WriteLine("+++Node List+++");
                            foreach (HostIPPort hIPP in network.allNodes)
                            {
                                Console.WriteLine(hIPP.getUrl() + "   ID:" + hIPP.getID().ToString());
                            }
                            Console.WriteLine("---Node List---");

                            //// run Bully Algorithm
                            //BullyAlgorithm.start();

                            //// if self is the master node, update variables and send messages to other nodes
                            //if (network.isSelfMasterNode == true)
                            //{
                            //    network.masterNodeFullUrlID = network.SelfIPPort.getUrlID();

                            //    foreach (HostIPPort hIPP in network.allNodes)
                            //    {
                            //        if (!hIPP.Equals(network.SelfIPPort))
                            //            Client.announceMasterByXMLRPC(hIPP.getUrl(), network.masterNodeFullUrlID);
                            //    }
                            //}
                        }// if (!network.idAssigned)

                        // run Bully Algorithm
                        if (network.masterNodeFullUrlID == "")
                        {
                            Console.WriteLine("\nNo master node, run Bully Algorithm...");
                            BullyAlgorithm.start();

                            // if self is the master node, update variables and send messages to other nodes
                            if (network.isSelfMasterNode == true)
                            {
                                network.masterNodeFullUrlID = network.SelfIPPort.getUrlID();

                                foreach (HostIPPort hIPP in network.allNodes)
                                {
                                    if (!hIPP.Equals(network.SelfIPPort))
                                        Client.announceMasterByXMLRPC(hIPP.getUrl(), network.masterNodeFullUrlID);
                                }
                            }
                        }

                        Console.WriteLine("\nUsing which algorithm: ");
                        Console.WriteLine("\"C\" for Centralised Mutual Exclusion   \"R\" for Ricart & Agrawala");
                        String input = Console.ReadLine();

                        //Thread.Sleep(500);

                        if (input != null)
                        {
                            if (input.Equals("C"))
                            {
                                Console.WriteLine("You are going to use " + "Centralised Mutual Exclusion.");
                                _mutualExclusionAlgorithm = 1;
                                foreach (HostIPPort h in network.allNodes)
                                {
                                    if (!h.Equals(network.SelfIPPort))
                                        Client.setAlgorithmAndStartedByXMLRPC(h.getUrl(), 1);
                                }
                            }
                            else if (input.Equals("R"))
                            {
                                Console.WriteLine("You are going to use " + "Ricart & Agrawala.");
                                _mutualExclusionAlgorithm = 2;
                                foreach (HostIPPort h in network.allNodes)
                                {
                                    if (!h.Equals(network.SelfIPPort))
                                        Client.setAlgorithmAndStartedByXMLRPC(h.getUrl(), 2);
                                }
                            }
                        }

                        network.Started = true;
                    } // if (!network.Started)
                    else
                    {
                        Console.WriteLine("Sending messages already started...");
                    }

                    return 1;
                } //if (network.Online)            
                else
                {
                    Console.WriteLine("Node signed off...");
                    return 0;
                }
            }
        }
    }
}
