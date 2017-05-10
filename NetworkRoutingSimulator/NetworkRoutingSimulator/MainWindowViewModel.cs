using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace NetworkRoutingSimulator
{
    class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            CreateGraphToVisualize();
            CreateRoutingGraph();
        }

        private IBidirectionalGraph<object, IEdge<object>> _graphToVisualize;
        private RoutingGraph _routingGraph;

        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get { return _graphToVisualize; }
        }

        public RoutingGraph RoutingGraph
        {
            get { return _routingGraph; }
        }

        private void CreateGraphToVisualize()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            string[] vertices = new string[5];
            for (int i = 0; i < 5; i++)
            {
                vertices[i] = i.ToString();
                g.AddVertex(vertices[i]);
            }

            g.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            g.AddEdge(new Edge<object>(vertices[1], vertices[2]));
            g.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            g.AddEdge(new Edge<object>(vertices[3], vertices[1]));
            g.AddEdge(new Edge<object>(vertices[4], vertices[0]));

            _graphToVisualize = g;
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
