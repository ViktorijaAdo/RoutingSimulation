using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace NetworkRoutingSimulator
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            _createRouterCommand = new RelayCommand(OnCreateRouterCommand, CanCreateRouter);
            _createConnectionCommand = new RelayCommand(OnCreateConnection, CanCreateConnection);
            CreateRoutingGraph();
        }

        private String _newRouterName;
        private RelayCommand _createRouterCommand;
        private RouterVertex _newConnectionTarget;
        private RouterVertex _newConnextionSource;
        private RelayCommand _createConnectionCommand;
        private RoutingGraph _routingGraph;

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

        public RoutingGraph RoutingGraph
        {
            get { return _routingGraph; }
        }

        private bool CanCreateRouter()
            {
            return NewRouterName != null && RoutingGraph.Vertices.Where(x => x.RouterName == NewRouterName).Count() == 0;
            }

        private void OnCreateRouterCommand()
            {
            RoutingGraph.AddVertex(new RouterVertex(_newRouterName));
            PropertyChanged(this, new PropertyChangedEventArgs("RoutingGraph.Vertices"));
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

        private void CreateRoutingGraph()
        {
            var g = new RoutingGraph();

            RouterVertex[] vertexes = new RouterVertex[5];
            for(int i = 0; i<5; i++)
            {
                vertexes[i] = new RouterVertex(i.ToString());
                g.AddVertex(vertexes[i]);
            }

            g.AddEdge(new ConnectionEdge(vertexes[0], vertexes[1]));
            g.AddEdge(new ConnectionEdge(vertexes[1], vertexes[2]));
            g.AddEdge(new ConnectionEdge(vertexes[2], vertexes[3]));
            g.AddEdge(new ConnectionEdge(vertexes[3], vertexes[1]));
            g.AddEdge(new ConnectionEdge(vertexes[4], vertexes[0]));

            _routingGraph = g;
        }
    }
}
