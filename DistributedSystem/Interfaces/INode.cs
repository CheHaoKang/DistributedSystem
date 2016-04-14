using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DistributedSystem
{
    public interface INode : IXmlRpcProxy
    {
        [XmlRpcMethod("Node.addNode")]
        bool addNode(string IPPort);

        [XmlRpcMethod("Node.signoffNode")]
        bool signoffNode(string IPPort);

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
    }
}
