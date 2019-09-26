using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.AbstractBuffSimulator;

namespace GW2EIParser.EIData
{
    public class ForceOverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffOverrideItem> overrides)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            BuffStackItem stack = stacks[0];
            overrides.Add(new BuffOverrideItem(stack.Src, stackItem.Src, stack.BoonDuration, stack.Start, stack.ID));
            if (stack.Extensions.Count > 0)
            {
                foreach ((AgentItem src, long value) in stack.Extensions)
                {
                    overrides.Add(new BuffOverrideItem(src, stackItem.Src, value, stack.Start, stack.ID));
                }
            }
            stacks[0] = stackItem;
            return true;
        }
    }
}
