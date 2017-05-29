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

            v.RoutingTableSent += delegate (RouterVertex sender, RouterVertex receiver, List<RouterVertex.RoutingInfo> routingTable)
            {
                if (ContainsVertex(sender) && ContainsVertex(receiver) && ContainsEdge(sender, receiver))
                    receiver.AddRoutingTableUpdate(sender, routingTable);
            };

            v.PackageSent += delegate (RouterVertex sender, RouterVertex receiver, NetworkPacket packet)
            {
                if (ContainsVertex(sender) && ContainsVertex(receiver) && ContainsEdge(sender, receiver))
                    receiver.SendPackage(packet);
                else
                    sender.ReturnFailedToSendPacket(packet);
            };
            return true;
            }
        }
}
