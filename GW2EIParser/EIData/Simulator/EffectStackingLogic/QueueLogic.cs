using System;
using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.AbstractBuffSimulator;

namespace GW2EIParser.EIData
{
    public class QueueLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffOverrideItem> overrides)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidOperationException("Queue logic based must have a >1 capacity");
            }
            BuffStackItem first = stacks[0];
            stacks.RemoveAt(0);
            BuffStackItem minItem = stacks.MinBy(x => x.TotalBoonDuration());
            if (minItem.TotalBoonDuration() > stackItem.TotalBoonDuration() + GeneralHelper.ServerDelayConstant)
            {
                stacks.Insert(0, first);
                return false;
            }
            overrides.Add(new BuffOverrideItem(minItem.Src, stackItem.Src, minItem.Duration, minItem.Start, minItem.ID));
            if (minItem.Extensions.Count > 0)
            {
                foreach ((AgentItem src, long value) in minItem.Extensions)
                {
                    overrides.Add(new BuffOverrideItem(src, stackItem.Src, value, minItem.Start, minItem.ID));
                }
            }
            stacks[stacks.IndexOf(minItem)] = stackItem;
            stacks.Insert(0, first);
            Sort(log, stacks);
            return true;
        }
    }
}

