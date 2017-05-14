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
        private ObservableCollection<NetworkPacket> _containingPackages;

        public RouterVertex(String routerName)
        {
            RouterName = routerName;
            _containingPackages = new ObservableCollection<NetworkPacket>();
        }

        public String RouterName { get; set; }

        public ObservableCollection<NetworkPacket> ContainingPackages
            {
            get
                {
                return _containingPackages;
                }
            }
    }
}
