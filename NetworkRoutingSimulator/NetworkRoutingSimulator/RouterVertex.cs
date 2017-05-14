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

        public delegate void DeletedEventHandler(object sender, EventArgs e);
        public event DeletedEventHandler Deleted = delegate { }; 

        private Dictionary<RouterVertex, List<RoutingInfo>> _neighborUpdates;

        private ObservableCollection<RouterVertex> _neighboards;

        private Dictionary<RouterVertex, int> _disaperedNeighbors;

        private ObservableCollection<RoutingInfo> _routingTable;

        private List<NetworkPacket> _NewPackages;

        private ObservableCollection<NetworkPacket> _containingPackages;

        private RelayCommand _deleteRouterCommand;

        public RouterVertex(String routerName)
        {
            RouterName = routerName;
            _containingPackages = new ObservableCollection<NetworkPacket>();
            _NewPackages = new List<NetworkPacket>();
            _routingTable = new ObservableCollection<RoutingInfo>() { new RoutingInfo() { DestinationName = RouterName, NextRouter = null, HopsNumber = 0} };
            _neighboards = new ObservableCollection<RouterVertex>();
            _neighborUpdates = new Dictionary<RouterVertex, List<RoutingInfo>>();
            _disaperedNeighbors = new Dictionary<RouterVertex, int>();
            _deleteRouterCommand = new RelayCommand(OnDelete);
            }

        public String RouterName { get; set; }

        public RelayCommand DeleteRouterCommand { get { return _deleteRouterCommand; } }
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

        public void SendPackage(NetworkPacket packet)
            {
            _NewPackages.Add(packet);
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
                if(!_neighborUpdates.ContainsKey(neighbor))
                    {
                    if (_disaperedNeighbors.ContainsKey(neighbor))
                        _disaperedNeighbors[neighbor]++;
                    else
                        _disaperedNeighbors.Add(neighbor, 1);

                    continue;
                    }

                var update = _neighborUpdates[neighbor];
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
                if (packet.DestinationRouterName == this.RouterName)
                    sentPackets.Add(packet);

                String nextRouter = RoutingTable.Where(x => x.DestinationName == packet.DestinationRouterName).Select(x => x.NextRouter).SingleOrDefault();
                if(nextRouter != null)
                    {
                    var neighbor = Neighboars.Where(x => x.RouterName == nextRouter).SingleOrDefault();
                    neighbor.SendPackage(packet);
                    sentPackets.Add(packet);
                    }
                }

            foreach (var packet in sentPackets)
                this.ContainingPackages.Remove(packet);
            }

        internal void CheckForDisconectedRouters()
            {
            foreach (var missingNeighbor in _disaperedNeighbors.Keys)
                {
                if (_disaperedNeighbors[missingNeighbor] == 6)
                    {
                    _neighboards.Remove(missingNeighbor);
                    var nonValidEntries = _routingTable.Where(x => x.DestinationName == missingNeighbor.RouterName || x.NextRouter == missingNeighbor.RouterName);
                    foreach(var entry in nonValidEntries)
                        {
                        _routingTable.Remove(entry);
                        foreach (var neighbor in _neighboards)
                            {
                            }
                        }
                    }
                }
            }

        public void UpdatePackets()
            {
            foreach(var packet in _NewPackages)
                {
                ContainingPackages.Add(packet);
                }

            _NewPackages = new List<NetworkPacket>();
            }

        private void OnDelete()
            {
            Deleted(this, new EventArgs());
            }

        }
}
