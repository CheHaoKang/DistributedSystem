using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DistributedSystem
{
    public interface IServerAgrawala : IXmlRpcProxy
    {
        [XmlRpcMethod("Agrawala.getRequest")]
        bool receiveRequest(int valueLamportClock, string IP, int Port);
        //bool getRequest(string nodeId, int requesterTimestamp);

        [XmlRpcMethod("Agrawala.getOk")]
        bool receiveOk(string IP, int Port);

        //[XmlRpcMethod("Agrawala.getTimestamp")]
        //int getTimestamp();
    }
}
