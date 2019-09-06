using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemIntensity : BuffSimulationItem
    {
        private readonly List<BuffSimulationItemDuration> _stacks = new List<BuffSimulationItemDuration>();

        public BuffSimulationItemIntensity(List<BoonStackItem> stacks) : base(stacks[0].Start, 0)
        {
            foreach (BoonStackItem stack in stacks)
            {
                var bstack = new BuffSimulationItemDuration(stack);
                _stacks.Add(bstack);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override void OverrideEnd(long end)
        {
            foreach (BuffSimulationItemDuration stack in _stacks)
            {
                stack.OverrideEnd(end);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override int GetTickingStacksCount()
        {
            return _stacks.Count;
        }

        public override void SetBoonDistributionItem(BuffDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log)
        {
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                item.SetBoonDistributionItem(distribs, start, end, boonid, log);
            }
        }


        public override List<JsonBuffStackStatus.JsonBuffStackStatusItem> GetStackStatusList()
        {
            var res = new List<JsonBuffStackStatus.JsonBuffStackStatusItem>();
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                res.Add(new JsonBuffStackStatus.JsonBuffStackStatusItem
                {
                    SourceId = item.Src.UniqueID
                });
            }
            return res;
        }
    }
}
