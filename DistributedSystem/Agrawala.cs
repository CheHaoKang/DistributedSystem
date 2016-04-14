using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DistributedSystem
{
    public class Agrawala
    {
        private static readonly object _mutualLock = new object();
        private Queue<HostIPPort> _queue = new Queue<HostIPPort>();
        private LamportClock _lamportClock = new LamportClock();
        private bool _accessingResource = false;
        private int _replicateClock = -1;  // -1 means not want to use
        private int _oKNum = 0;


        public LamportClock lamportClock
        {
            get
            {
                return _lamportClock;
            }
            set
            {
                lock (_mutualLock)
                {
                    _lamportClock = value;
                }
            }
        }

        public int replicateLock
        {
            get
            {
                return _replicateClock;
            }
            set
            {
                lock (_mutualLock)
                {
                    _replicateClock = value;
                }
            }
        }

        public int okNum
        {
            get
            {
                return _oKNum;
            }
            set
            {
                lock (_mutualLock)
                {
                    _oKNum = value;
                }
            }
        }

        public bool accessingResource
        {
            get
            {
                return _accessingResource;
            }
            set
            {
                lock (_mutualLock)
                {
                    _accessingResource = value;
                }
            }
        }

        public Queue<HostIPPort> queue
        {
            get
            {
                return _queue;
            }
            set
            {
                lock (_mutualLock)
                {
                    _queue = value;
                }
            }
        }

        public Agrawala()
        {
        }

        public bool sendRequest()
        {
            Network network = Network.Instance;
            Agrawala agrawala = Network.agrawala;

            Console.WriteLine("### " + network.SelfIPPort.getUrlID() + " is sending the request... ###");
            agrawala.lamportClock.change();

            agrawala.replicateLock = agrawala.lamportClock.getCurrentLamportClock();
            Console.WriteLine("### Timestamp is: " + agrawala.replicateLock + "###");

            agrawala.okNum = 0;

            foreach (HostIPPort h in network.allNodes)
            {
                if (!h.Equals(network.SelfIPPort))
                {

                    Console.WriteLine("### Send request to " + h.getUrlID() + "###");
                    sendRequestByXMLRPC(h, agrawala.replicateLock);
                }
            }

            Console.WriteLine("### Ok: " + agrawala.okNum + "###");
            Console.WriteLine("### All nodes: " + network.allNodes.Count() + "###");

            if (agrawala.okNum < network.allNodes.Count()-1)
            {
                Console.WriteLine("===== Wait for entering =====");
            }

            int ok = 0;
            int okNeeded = network.allNodes.Count()-1;
            while (ok < okNeeded)
            {
                lock (_mutualLock)
                {
                    ok = agrawala.okNum;
                }
            }
            agrawala.accessingResource = true;
            Console.WriteLine("===== Entering =====");

            return true;
        }

        public bool getRequest(int senderLamportClock, string senderIp, int senderPort)
        {
            lock (_mutualLock)
            {
                Network network = Network.Instance;
                Agrawala agrawala = Network.agrawala;

                Console.WriteLine("### Receive reqeust from " + senderIp + ":" + senderPort + " ###");
                agrawala.lamportClock.receiveRequest(senderLamportClock);

                HostIPPort sender = new HostIPPort(senderIp, senderPort);

                int senderId = 0;
                foreach (HostIPPort hIPP in network.allNodes)
                {
                    if (hIPP.getUrl().Equals(sender.getUrl()))
                    {
                        senderId = hIPP.getID();
                        break;
                    }
                }

                if (agrawala.accessingResource == true)
                {
                    Console.WriteLine("### Using resource: " + agrawala.accessingResource + "###");
                    Console.WriteLine("### Adding node: " + senderIp + " :" + senderPort + " to the queue...###");
                    agrawala.queue.Enqueue(sender);
                }
                else if ((senderLamportClock == agrawala.replicateLock && (senderId < network.SelfIPPort.getID())) || (senderLamportClock < agrawala.replicateLock) || (agrawala.replicateLock == -1))
                {
                    Console.WriteLine("### Send OK to: " + sender.getUrlID() + "###");
                    sendOKByXMLRPC(sender, agrawala.lamportClock.getCurrentLamportClock());
                }
                else
                {
                    Console.WriteLine("### Timestamp is " + agrawala.replicateLock + "###");
                    Console.WriteLine("### Sender timestamp is " + senderLamportClock + "###");
                    Console.WriteLine("### Add node: " + senderIp + ":" + senderPort + " to the queue ###");
                    agrawala.queue.Enqueue(sender);
                }

                return true;
            }
        }

        public bool getOk(string IP, int Port)
        {
            lock (_mutualLock)
            {
                Console.WriteLine("### OK sent from: " + IP + ":" + Port + "###");

                Network network = Network.Instance;
                Agrawala agrawala = Network.agrawala;
                agrawala.okNum += 1;

                Console.WriteLine("### Number of OK: " + agrawala.okNum + "###");

                if (agrawala.okNum == network.allNodes.Count() - 1)
                {
                    agrawala.accessingResource = true;
                    Console.WriteLine("### Ok is enough. Go to use the resource...OK: " + agrawala.okNum + "###");
                }

                return true;
            }
        }

        public void releaseResource()
        {
            Network network = Network.Instance;
            Agrawala agrawala = Network.agrawala;
            
            Console.WriteLine("### Release resource... ###");
            agrawala.replicateLock = -1;
            agrawala.accessingResource = false;

            while (agrawala.queue.Count() != 0)
            {
                HostIPPort dequeue;
                lock (_mutualLock)
                {
                    dequeue = agrawala.queue.Dequeue();
                }
                Console.WriteLine(dequeue);
                sendOKByXMLRPC(dequeue, agrawala.lamportClock.getCurrentLamportClock());
            }
        }

        public void sendOKByXMLRPC(HostIPPort host, int clockValue)
        {
            Network network = Network.Instance;
            Agrawala agrawala = Network.agrawala;
            IServerAgrawala proxy = XmlRpcProxyGen.Create<IServerAgrawala>();
            proxy.Url = host.getUrl();
            proxy.receiveOk(network.SelfIPPort.getIP(), network.SelfIPPort.getPort());
        }

        public void sendRequestByXMLRPC(HostIPPort host, int valueLamportClock)
        {
            Network network = Network.Instance;
            IServerAgrawala proxy = XmlRpcProxyGen.Create<IServerAgrawala>();
            proxy.Url = host.getUrl();
            proxy.receiveRequest(valueLamportClock, network.SelfIPPort.getIP(), network.SelfIPPort.getPort());
        }
    }
}
