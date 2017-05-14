using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace NetworkRoutingSimulator
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            _createRouterCommand = new RelayCommand(OnCreateRouterCommand, CanCreateRouter);
            _createConnectionCommand = new RelayCommand(OnCreateConnection, CanCreateConnection);
            _createPacketCommand = new RelayCommand(OnCreatePacket, CanCreatePacket);
            _doRoutingStepCommand = new RelayCommand(OnRoutingStep);
            _vertices = null;
            CreateRoutingGraph();
        }

        private String _newRouterName;
        private RelayCommand _createRouterCommand;
        private RouterVertex _newConnectionTarget;
        private RouterVertex _newConnextionSource;
        private RelayCommand _createConnectionCommand;
        private RouterVertex _newPacketSource;
        private RouterVertex _newPacketDestination;
        private RelayCommand _createPacketCommand;
        private RelayCommand _doRoutingStepCommand;
        private RoutingGraph _routingGraph;
        private ObservableCollection<RouterVertex> _vertices;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public String NewRouterName
            {
            get
                {
                return _newRouterName;
                }
            set
                {
                _newRouterName = value;
                CreateRouterCommand.RaiseCanExecuteChanged();
                }
            }
        public RelayCommand CreateRouterCommand { get { return _createRouterCommand; } }

        public RouterVertex NewConnectionTarget
            {
            get
                {
                return _newConnectionTarget;
                }
            set
                {
                _newConnectionTarget = value;
                CreateConnectionCommand.RaiseCanExecuteChanged();
                }
            }

        public RouterVertex NewConnectionSource
            {
            get
                {
                return _newConnextionSource;
                }
            set
                {
                _newConnextionSource = value;
                CreateConnectionCommand.RaiseCanExecuteChanged();
                }
            }

        public RelayCommand CreateConnectionCommand { get { return _createConnectionCommand; } }

        public RelayCommand DoRoutingStepCommand { get { return _doRoutingStepCommand; } }

        public RoutingGraph RoutingGraph
        {
            get { return _routingGraph; }
        }

        public ObservableCollection<RouterVertex> Vertices
            {
            get {
                if (_vertices == null)
                    return _vertices = new ObservableCollection<RouterVertex>(RoutingGraph.Vertices);
                return _vertices;
                }
            }

        public RouterVertex NewPacketSource
            {
            get
                {
                return _newPacketSource;
                }
            set
                {
                _newPacketSource = value;
                CreatePacketCommand.RaiseCanExecuteChanged();
                }
            }
        public RouterVertex NewPacketDestination
            {
            get
                {
                return _newPacketDestination;
                }
            set
                {
                _newPacketDestination = value;
                CreatePacketCommand.RaiseCanExecuteChanged();
                }
            }

        public RelayCommand CreatePacketCommand { get { return _createPacketCommand; } }

        private bool CanCreateRouter()
            {
            return NewRouterName != null && RoutingGraph.Vertices.Where(x => x.RouterName == NewRouterName).Count() == 0;
            }

        private void OnCreateRouterCommand()
            {
            var router = new RouterVertex(_newRouterName);
            RoutingGraph.AddVertex(router);
            if (_vertices != null)
                _vertices.Add(router);
            }

        private bool CanCreateConnection()
            {
            return NewConnectionSource != null && NewConnectionTarget != null && NewConnectionSource != NewConnectionTarget;
            }

        private void OnCreateConnection()
            {
            RoutingGraph.AddEdge(new ConnectionEdge(NewConnectionSource, NewConnectionTarget));
            RoutingGraph.AddEdge(new ConnectionEdge(NewConnectionTarget, NewConnectionSource));
            }


        private bool CanCreatePacket()
            {
            return NewPacketDestination != null && NewPacketSource != null;
            }

        private void OnCreatePacket()
            {
            NewPacketSource.ContainingPackages.Add(new NetworkPacket(NewPacketDestination.RouterName));
            }

        private void OnRoutingStep()
            {
            foreach (var router in RoutingGraph.Vertices)
                router.CheckForDisconectedRouters();

            foreach(var router in RoutingGraph.Vertices)
                router.SendRoutingTableUpdate();

            foreach (var router in RoutingGraph.Vertices)
                router.UpdateRoutingTable();

            foreach(var router in RoutingGraph.Vertices)
                router.SendPackets();

            foreach (var Router in RoutingGraph.Vertices)
                Router.UpdatePackets();
            }

        private void CreateRoutingGraph()
        {
            var g = new RoutingGraph();

            RouterVertex[] vertexes = new RouterVertex[5];
            for(int i = 0; i<5; i++)
            {
                vertexes[i] = new RouterVertex(i.ToString());
                g.AddVertex(vertexes[i]);
            }
            vertexes[3].ContainingPackages.Add(new NetworkPacket("4"));
            vertexes[3].ContainingPackages.Add(new NetworkPacket("4"));

            g.AddEdge(new ConnectionEdge(vertexes[0], vertexes[1]));
            g.AddEdge(new ConnectionEdge(vertexes[1], vertexes[2]));
            g.AddEdge(new ConnectionEdge(vertexes[2], vertexes[3]));
            g.AddEdge(new ConnectionEdge(vertexes[3], vertexes[1]));
            g.AddEdge(new ConnectionEdge(vertexes[4], vertexes[0]));

            _routingGraph = g;
        }
    }
}
