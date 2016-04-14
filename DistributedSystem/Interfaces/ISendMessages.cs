using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DistributedSystem
{
    public interface ISendMessages : IXmlRpcProxy
    {
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

        //[XmlRpcMethod("SendMessages.Substract")]
        //int Subtract(int a, int b);
    }
}
