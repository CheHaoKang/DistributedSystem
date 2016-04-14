using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DistributedSystem
{
    public interface IServiceContract
    {
        [XmlRpcMethod("Node.addNode")]
        bool addNode(string IpnPort);

        [XmlRpcMethod("Node.signoffNode")]
        bool signoffNode(string IpnPort);

        [XmlRpcMethod("Node.getAllNodes")]
        string[] getAllNodes();

        [XmlRpcMethod("Node.setId")]
        bool setNodeId(string[] nodesIPPortID);

        [XmlRpcMethod("Node.bullyAlgorithm")]
        int bullyAlgorithm(string fullUrlID); // if return 1 means the higher ID node sent OK and wants to take over

        [XmlRpcMethod("Node.announceMaster")]
        bool announceMaster(string masterFullUrlID);

        //[XmlRpcMethod("Node.start")]
        //bool start(string messageToBeSent);

        [XmlRpcMethod("Node.setAlgorithmAndStarted")]
        bool setAlgorithmAndStarted(int algorithm);

        // Centralized Mutual Exclusion
        [XmlRpcMethod("SendMessages.requestToMaster")]
        string[] requestToMaster(string nodeFullUrlID);

        //[XmlRpcMethod("SendMessages.replyToClient")]
        //void replyToClient(string repliedString);

        [XmlRpcMethod("SendMessages.appendStringToMaster")]
        string appendStringToMaster(string nodeFullUrlID, string appendedString);

        [XmlRpcMethod("SendMessages.setWaitFlag")]
        bool setWaitFlag(bool waitFlag, string masterString);

        [XmlRpcMethod("SendMessages.getMasterString")]
        string getMasterString();

        [XmlRpcMethod("SendMessages.notifyFinished")]
        void notifyFinished(string node);

        // Agrawala Mutual Exclusion
        [XmlRpcMethod("Agrawala.getRequest")]
        bool receiveRequest(int valueLamportClock, string IP, int Port);
        //bool getRequest(string nodeId, int requesterTimestamp);

        [XmlRpcMethod("Agrawala.getOk")]
        bool receiveOk(string IP, int Port);
    }
}
