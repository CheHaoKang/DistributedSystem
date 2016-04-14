using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSystem
{
    public class HostIPPort
    {
        private string IP;
        private int port;
        private int ID;

        public HostIPPort(string ip, int port)
        {
            IP = ip;
            this.port = port;
        }

        public HostIPPort(string IPPort)
        {
            string[] strings = IPPort.Split(':');
            IP = strings[0];
            port = Convert.ToInt32(strings[1]);
        }

        public void setIP(string ip)
        {
            IP = ip;
        }

        public string getIP()
        {
            return IP;
        }
        
        public int getPort()
        {
            return port;
        }

        public void setPort(int port)
        {
            this.port = port;
        }

        public void setID(int id)
        {
            ID = id;
        }

        public int getID()
        {
            return ID;
        }

        public string getIPPort()
        {
            return IP + ":" + this.port;
        }

        public string getUrl()
        {
            return "http://" + IP + ":" + port + "/xmlrpc";
        }

        public string getUrlID()
        {
            return getUrl() + "_" + ID;
        }

        public string toString()
        {
            return "http://" + IP + ":" + port + "/";
        }

        public long getHostValue()
        {
            string[] strings = IP.Split('.');
            string ID = "";

            for (int i = 0; i < strings.Length; i++)
                ID += strings[i];

            ID += port; // return Concat(arg0.ToString(), arg1.ToString());
            return Convert.ToInt64(ID);
        }

        public int hostIPComparison(HostIPPort hIPP2)
        {
            if (this.getHostValue() == hIPP2.getHostValue())
                return 0;
            if (this.getHostValue() < hIPP2.getHostValue())
                return -1;
            else
                return 1;
        }
    }
}
