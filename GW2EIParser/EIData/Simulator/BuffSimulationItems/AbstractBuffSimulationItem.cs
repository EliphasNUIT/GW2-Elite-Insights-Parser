using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using System.Collections.Generic;

namespace GW2EIParser.EIData
{
    public abstract class AbstractBuffSimulationItem
    {

        protected Dictionary<AgentItem, BuffDistributionItem> GetDistrib(BuffDistribution distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out var distrib))
            {
                distrib = new Dictionary<AgentItem, BuffDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBoonDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log);
    }
}
