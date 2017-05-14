using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRoutingSimulator
    {
    class NetworkPacket
        {
        public NetworkPacket(String destinationRouterName)
            {
            DestinationRouterName = destinationRouterName;
            }
        public String DestinationRouterName { get; private set; }
        }
    }
