using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;

namespace NetworkRoutingSimulator
{
    class RoutingGraph : BidirectionalGraph<RouterVertex, ConnectionEdge>
    {
        public override bool AddVertex(RouterVertex v)
            {
            base.AddVertex(v);
            v.Deleted += delegate (object sender, EventArgs e)
                {
                var router = v as RouterVertex;
                this.RemoveVertex(router);
                };
            return true;
            }
        }
}
