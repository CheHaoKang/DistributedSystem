using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace DistributedSystem
{
    class Server : MarshalByRefObject , IServiceContract
    {
        private static int _port;
        static HttpChannel _channel;

        public Server(int port)
        {
            _port = port;
        }

        public Server()
        {
        }

        public void stopServer()
        {
            ChannelServices.UnregisterChannel(_channel);
            Thread.Sleep(150);
        }

        public void execute()
        {
            try
            {
                Console.WriteLine("Server is on...");
                IDictionary properties = new Hashtable();
                properties["name"] = "ServerHttpChannel";
                properties["port"] = _port;
                _channel = new HttpChannel(
                   properties,
                   null,
                   new XmlRpcServerFormatterSinkProvider()
                );
                ChannelServices.RegisterChannel(_channel, false);

                RemotingConfiguration.RegisterWellKnownServiceType(
                  typeof(Server),
                  "xmlrpc",  // HostIPPort.cs - getUrl - return "http://" + IP + ":" + port + "/xmlrpc";
                  //WellKnownObjectMode.Singleton); // Every incoming message is serviced by the same object instance.
                  WellKnownObjectMode.SingleCall); // Every incoming message is serviced by a new object instance.
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool addNode(string IPPort)
        {
            Node node = new Node();
            return node.addNode(IPPort);
        }

        public bool signoffNode(string IPPort)
        {
            Node node = new Node();
            return node.signoffNode(IPPort);
        }

        public string[] getAllNodes()
        {
            Node node = new Node();
            return node.getAllNodes();
        }

        public bool setNodeId(string[] nodesIPPortID)
        {
            Node node = new Node();
            return node.setNodeId(nodesIPPortID);
        }

        public int bullyAlgorithm(string fullUrlID)
        {
            Node node = new Node();
            return node.bullyAlgorithm(fullUrlID);
        }

        public bool announceMaster(string masterFullUrlID)
        {
            Node node = new Node();
            return node.announceMaster(masterFullUrlID);
        }

        //public bool start(string messageToBeSent)
        //{
        //    Node node = new Node();
        //    return node.start(messageToBeSent);
        //}

        public bool setAlgorithmAndStarted(int algorithm)
        {
            Node node = new Node();
            return node.setAlgorithmAndStarted(algorithm);
        }

        public string[] requestToMaster(string nodeFullUrlID)
        {
            SendMessages sendM = new SendMessages();
            return sendM.requestToMaster(nodeFullUrlID);
        }

        //public void replyToClient(string repliedString)
        //{
        //}

        public string appendStringToMaster(string nodeFullUrlID, string appendedString)
        {
            SendMessages sendM = new SendMessages();
            return sendM.appendStringToMaster(nodeFullUrlID, appendedString);
        }

        public bool setWaitFlag(bool waitFlag, string masterString)
        {
            SendMessages sendM = new SendMessages();
            sendM.setWaitFlag(waitFlag, masterString);
            return true;
        }

        public string getMasterString()
        {
            SendMessages sendM = new SendMessages();
            return sendM.getMasterString();
        }

        public void notifyFinished(string node)
        {
            SendMessages sendM = new SendMessages();
            sendM.notifyFinished(node);
        }

        // Agrawala
        public bool receiveOk(string IP, int Port)
        {
            Agrawala agrawa = Network.agrawala;
            agrawa.getOk(IP, Port);

            return true;
        }

        public bool receiveRequest(int valueLamportClock, string IP, int Port)
        {
            Agrawala agrawa = Network.agrawala;
            return agrawa.getRequest(valueLamportClock, IP, Port);
        }

        //public int getTimestamp()
        //{
        //    Agrawala agrawa = Agrawala.Instance;
        //    return agrawa.Timestamp;
        //}
    }
}
