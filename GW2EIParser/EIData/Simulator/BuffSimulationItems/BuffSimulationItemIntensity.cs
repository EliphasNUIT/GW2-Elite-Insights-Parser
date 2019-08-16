using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemIntensity : BuffSimulationItem
    {
        private List<BuffSimulationItemDuration> _stacks = new List<BuffSimulationItemDuration>();
        private List<AgentItem> _sources;

        public BuffSimulationItemIntensity(List<BoonStackItem> stacks) : base(stacks[0].Start, 0)
        {
            foreach (BoonStackItem stack in stacks)
            {
                _stacks.Add(new BuffSimulationItemDuration(stack));
            }
            Duration = _stacks.Max(x => x.Duration);
            _sources = new List<AgentItem>();
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                _sources.AddRange(item.GetSources());
            }
        }

        public override void SetEnd(long end)
        {
            foreach (BuffSimulationItemDuration stack in _stacks)
            {
                stack.SetEnd(end);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override int GetStack()
        {
            return _stacks.Count;
        }

        public override void SetBoonDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                item.SetBoonDistributionItem(distribs, start, end, boonid, log);
            }
        }

        public override List<AgentItem> GetSources()
        {
            return _sources;
        }
    }
}
