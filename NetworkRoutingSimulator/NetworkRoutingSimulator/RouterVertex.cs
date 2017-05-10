using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRoutingSimulator
{
    class RouterVertex
    {
        public RouterVertex(String routerName)
        {
            RouterName = routerName;
        }

        public String RouterName
        {
            get; set;
        }
    }
}
