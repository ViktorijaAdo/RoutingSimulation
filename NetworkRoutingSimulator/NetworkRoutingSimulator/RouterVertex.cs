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
        public static int INFINITY = 16;

        public class RoutingInfo
            {
            public String DestinationName { get; set; }
            public String NextRouter { get; set; }
            public long HopsNumber { get; set; }
            }

        public delegate void DeletedEventHandler(object sender, EventArgs e);
        public event DeletedEventHandler Deleted = delegate { };

        public delegate void SentRoutingTableEventHandler(RouterVertex sender, RouterVertex receiver, List<RoutingInfo> routingTable);
        public event SentRoutingTableEventHandler RoutingTableSent = delegate { };

        public delegate void SentPackageEventHandler(RouterVertex sender, RouterVertex receiver, NetworkPacket packet);
        public event SentPackageEventHandler PackageSent = delegate { };

        private Dictionary<RouterVertex, List<RoutingInfo>> _neighborUpdates;

        private ObservableCollection<RouterVertex> _neighboards;

        private Dictionary<RouterVertex, int> _disaperedNeighbors;

        private ObservableCollection<RoutingInfo> _routingTable;

        private List<NetworkPacket> _NewPackages;

        private ObservableCollection<NetworkPacket> _containingPackages;

        private RelayCommand _deleteRouterCommand;

        private List<NetworkPacket> _sentPackets = new List<NetworkPacket>();

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

        public void AddRoutingTableUpdate(RouterVertex sender, List<RoutingInfo> routingTable)
        {
            _neighborUpdates.Add(sender, routingTable);
        }

        public void SendPackage(NetworkPacket packet)
            {
            _NewPackages.Add(packet);
            }

        public void ReturnFailedToSendPacket(NetworkPacket packet)
        {
            _sentPackets.Remove(packet);
        }

        public void RemoveConnection(RouterVertex neighborToRemove)
        {
            Neighboars.Remove(neighborToRemove);
            foreach(var info in RoutingTable)
            {
                if (info.NextRouter == neighborToRemove.RouterName)
                    info.HopsNumber = INFINITY;
            }
        }

        public void SendRoutingTableUpdate()
            {
            foreach(var neighbor in Neighboars)
                {
                RoutingTableSent(this, neighbor, RoutingTable.ToList());
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
                    if (selectedInfo == null && routerinfo.HopsNumber < INFINITY)
                        {
                        RoutingTable.Add(new RoutingInfo() { DestinationName = routerinfo.DestinationName, NextRouter = neighbor.RouterName, HopsNumber = routerinfo.HopsNumber >= INFINITY? INFINITY : routerinfo.HopsNumber + 1 });
                        }
                    else if(neighbor.RouterName == selectedInfo.NextRouter)
                    {
                        selectedInfo.HopsNumber = routerinfo.HopsNumber >= INFINITY ? INFINITY : routerinfo.HopsNumber +1;
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
            foreach (var packet in ContainingPackages)
                {
                if (packet.DestinationRouterName == this.RouterName)
                {
                    _sentPackets.Add(packet);
                    continue;
                }

                RoutingInfo sendingInfo = RoutingTable.Where(x => x.DestinationName == packet.DestinationRouterName).SingleOrDefault();
                if(sendingInfo != null && sendingInfo.HopsNumber < INFINITY)
                    {
                    var neighbor = Neighboars.Where(x => x.RouterName == sendingInfo.NextRouter).SingleOrDefault();
                    _sentPackets.Add(packet);
                    PackageSent(this, neighbor, packet);
                    }
                }

            foreach (var packet in _sentPackets)
                this.ContainingPackages.Remove(packet);
            }

        public void CheckForDisconectedRouters()
        {
            List<RouterVertex> neighboarsToRemove = new List<RouterVertex>();
            foreach (var missingNeighbor in _disaperedNeighbors.Keys)
            {
                if (_disaperedNeighbors[missingNeighbor] == 6)
                {
                    neighboarsToRemove.Add(missingNeighbor);
                }
            }
            foreach (var neighboar in neighboarsToRemove)
            {
                Neighboars.Remove(neighboar);
                foreach(var info in RoutingTable.Where(x => x.NextRouter == neighboar.RouterName))
                    info.HopsNumber = INFINITY;

                foreach (var validNeighboar in Neighboars)
                {
                    RoutingTableSent(this, validNeighboar, RoutingTable.ToList());
                    validNeighboar.UpdateRoutingTable();
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
