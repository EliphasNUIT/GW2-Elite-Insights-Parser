using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemQueue : BuffSimulationItemDuration
    {

        private readonly List<BuffSimulationItemDuration> _queue = new List<BuffSimulationItemDuration>();

        public BuffSimulationItemQueue(List<BoonStackItem> queue) : base(queue.First())
        {
            for (int i = 1; i < queue.Count; i++)
            {
                var stack = new BuffSimulationItemDuration(queue[i]);
                _queue.Add(stack);
            }
        }

        public override List<JsonBuffStackStatus.JsonBuffStackStatusItem> GetStackStatusList()
        {
            var res = new List<JsonBuffStackStatus.JsonBuffStackStatusItem>()
            {
                new JsonBuffStackStatus.JsonBuffStackStatusItem{SourceId = Src.UniqueID}
            };
            foreach (BuffSimulationItemDuration item in _queue)
            {
                res.Add(new JsonBuffStackStatus.JsonBuffStackStatusItem { SourceId = item.Src.UniqueID, Duration = item.Duration });
            }
            return res;
        }
    }
}
