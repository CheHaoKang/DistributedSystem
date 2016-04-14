using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DistributedSystem
{
     public sealed class Master
    {
        private static readonly Master _master = new Master();
        private string _occupied = "";
        private int _finishedNodes = 0;
        private static string _masterString = "";
        private Queue<string> _nodeQueue = new Queue<string>();
        private static readonly object _mutualLock = new object();

        private Master()
        {
        }

        public static Master Instance
        {
            get
            {
                return _master;
            }
        }

        public static object mutualLock
        {
            get
            {
                return _mutualLock;
            }
        }

        public string masterString 
        {
            get
            {
                return _masterString;
            }
            set
            {
                lock (_mutualLock)
                {
                    _masterString = value;
                }
            }
        }

        public Queue<string> nodeQueue
        {
            get
            {
                return _nodeQueue;
            }
            set
            {
                lock (_mutualLock)
                {
                    _nodeQueue = value;
                }
            }
        }

        public string occupied
        {
            get
            {
                return _occupied;
            }
            set
            {
                lock (_mutualLock)
                {
                    _occupied = value;
                }
            }
        }

        public int finishedNodes
        {
            get
            {
                return _finishedNodes;
            }
            set
            {
                lock (_mutualLock)
                {
                    _finishedNodes = value;
                }
            }
        }
     }
}
