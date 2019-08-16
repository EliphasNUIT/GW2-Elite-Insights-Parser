using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class ForceOverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            BoonStackItem stack = stacks[0];
            wastes.Add(new BuffSimulationItemWasted(stack.Src, stack.BoonDuration, stack.Start));
            if (stack.Extensions.Count > 0)
            {
                foreach ((AgentItem src, long value) in stack.Extensions)
                {
                    wastes.Add(new BuffSimulationItemWasted(src, value, stack.Start));
                }
            }
            stacks[0] = stackItem;
            return true;
        }
    }
}
