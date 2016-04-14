using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSystem
{
    public class LamportClock
    {
        private int lamportClock;

        public LamportClock()
        {
            lamportClock = 1;
        }

        public void reset()
        {
            lamportClock = 1;
        } 

        public void receiveRequest(int otherTimeStamp)
        {
            this.lamportClock = (this.lamportClock > otherTimeStamp) ? this.lamportClock + 1 : otherTimeStamp + 1;
        }

        public void change()
        {
            lamportClock += 1;
        }

        public int getCurrentLamportClock()
        {
            return lamportClock;
        }
    }
}
