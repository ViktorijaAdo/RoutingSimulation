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
        public class RoutingInfo
            {
            public String DestinationName { get; set; }
            public String NextRouter { get; set; }
            public long HopsNumber { get; set; }
            }

        private Dictionary<RouterVertex, List<RoutingInfo>> _neighborUpdates;

        private ObservableCollection<RouterVertex> _neighboards;

        private Dictionary<RouterVertex, int> _disaperedNeighbors;

        private ObservableCollection<RoutingInfo> _routingTable;

        private ObservableCollection<NetworkPacket> _containingPackages;

        public RouterVertex(String routerName)
        {
            RouterName = routerName;
            _containingPackages = new ObservableCollection<NetworkPacket>();
            _routingTable = new ObservableCollection<RoutingInfo>();
            _neighboards = new ObservableCollection<RouterVertex>();
            _neighborUpdates = new Dictionary<RouterVertex, List<RoutingInfo>>();
            _disaperedNeighbors = new Dictionary<RouterVertex, int>();
        }

        public String RouterName { get; set; }

        public ObservableCollection<RouterVertex> Neighboars { get { return _neighboards; } }

        public ObservableCollection<RoutingInfo> RoutingTable { get { return _routingTable; } }

        public ObservableCollection<NetworkPacket> ContainingPackages { get { return _containingPackages; } }

        public void AddNeighboard(RouterVertex neighboard)
            {
            if (Neighboars.Contains(neighboard))
                return;

            Neighboars.Add(neighboard);
            var info = RoutingTable.Where(x => x.DestinationName == neighboard.RouterName).SingleOrDefault();
            if (info == null)
                {
                info = new RoutingInfo() { DestinationName = neighboard.RouterName };
                RoutingTable.Add(info);
                }

            info.NextRouter = neighboard.RouterName;
            info.HopsNumber = 1;
            }

        public void SendRoutingTableUpdate()
            {
            foreach(var neighbor in Neighboars)
                {
                neighbor._neighborUpdates.Add(this, RoutingTable.ToList());
                }
            }

        public void UpdateRoutingTable()
            {
            foreach (var neighbor in Neighboars)
                {
                var update = _neighborUpdates[neighbor];
                if (update == null)
                    {
                    _disaperedNeighbors[neighbor]++;
                    continue;
                    }

                if (_disaperedNeighbors.ContainsKey(neighbor))
                    _disaperedNeighbors.Remove(neighbor);

                foreach (var routerinfo in update)
                    {
                    var selectedInfo = RoutingTable.Where(x => x.DestinationName == routerinfo.DestinationName).SingleOrDefault();
                    if (selectedInfo == null)
                        {
                        RoutingTable.Add(new RoutingInfo() { DestinationName = routerinfo.DestinationName, NextRouter = neighbor.RouterName, HopsNumber = routerinfo.HopsNumber + 1 });
                        }
                    else if (routerinfo.HopsNumber + 1 < selectedInfo.HopsNumber)
                        {
                        selectedInfo.HopsNumber = routerinfo.HopsNumber + 1;
                        selectedInfo.NextRouter = neighbor.RouterName;
                        }
                    }
                }
            _neighborUpdates = new Dictionary<RouterVertex, List<RoutingInfo>>();
            }

        public void SendPackets()
            {
            var sentPackets = new List<NetworkPacket>();
            foreach (var packet in ContainingPackages)
                {
                String nextRouter = RoutingTable.Where(x => x.DestinationName == packet.DestinationRouterName).Select(x => x.NextRouter).SingleOrDefault();
                if(nextRouter != null)
                    {
                    var neighbor = Neighboars.Where(x => x.RouterName == nextRouter).SingleOrDefault();
                    neighbor.ContainingPackages.Add(packet);
                    sentPackets.Add(packet);
                    }
                }

            foreach (var packet in sentPackets)
                this.ContainingPackages.Remove(packet);
            }

        }
}
