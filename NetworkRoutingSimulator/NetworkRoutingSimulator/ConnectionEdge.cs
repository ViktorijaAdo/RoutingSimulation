using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace NetworkRoutingSimulator
{
    class ConnectionEdge : IEdge<RouterVertex>
    {
        private RouterVertex _source;
        private RouterVertex _target;

        public RouterVertex Source { get {return _source; } }

        public RouterVertex Target { get { return _target; } }

        public ConnectionEdge(RouterVertex source, RouterVertex target)
        {
            _source = source;
            _target = target;
            Source.AddNeighboard(Target);
            Target.AddNeighboard(Source);
        }
    }
}
