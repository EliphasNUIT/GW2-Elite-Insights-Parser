using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;
using static GW2EIParser.Builders.JsonModels.JsonLog;
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

        public override List<object> GetStackStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            var res = new List<object>();
            var running = new List<object> { GetActorID(Src, log, description) };
            if (IsExtension)
            {
                running.Add(GetActorID(SeedSrc, log, description));
            }
            res.Add(running);
            foreach (BuffSimulationItemDuration item in _queue)
            {
                var subRes = new List<object>() { GetActorID(item.Src, log, description) };
                if (item.IsExtension)
                {
                    subRes.Add(GetActorID(item.SeedSrc, log, description));
                }
                subRes.Add(Duration);
                res.Add(subRes);
            }
            return res;
        }
    }
}
