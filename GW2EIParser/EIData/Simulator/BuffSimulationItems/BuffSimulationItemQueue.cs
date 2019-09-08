using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;
using static GW2EIParser.Builders.JsonModels.JsonBuffData.JsonBuffStackStatus;
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

        public override List<JsonBuffStackStatusSources> GetStackStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            var res = new List<JsonBuffStackStatusSources>();
            var running = new JsonBuffStackStatusSources { Src = GetActorID(Src, log, description) };
            if (IsExtension)
            {
                running.SeedSrc = GetActorID(SeedSrc, log, description);
            }
            res.Add(running);
            foreach (BuffSimulationItemDuration item in _queue)
            {
                var subRes = new JsonBuffStackStatusSources {Src = GetActorID(item.Src, log, description) };
                if (item.IsExtension)
                {
                    subRes.SeedSrc = GetActorID(item.SeedSrc, log, description);
                }
                subRes.Duration = Duration;
                res.Add(subRes);
            }
            return res;
        }
    }
}
