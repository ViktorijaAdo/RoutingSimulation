using System;
using System.Data;
using System.Collections.ObjectModel;
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
            RoutingTable = new DataTable();
            RoutingTable.NewRow();
        }

        public DataTable RoutingTable { get; set; }

        public String RouterName
        {
            get; set;
        }
    }
}
