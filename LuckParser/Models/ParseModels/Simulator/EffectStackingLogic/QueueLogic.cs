﻿using LuckParser.Controllers;
using LuckParser.Parser;
using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class QueueLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidOperationException("Queue logic based must have a >1 capacity");
            }
            BoonStackItem first = stacks[0];
            stacks.RemoveAt(0);
            BoonStackItem minItem = stacks.MinBy(x => x.TotalBoonDuration());
            if (minItem.TotalBoonDuration() >= stackItem.TotalBoonDuration())
            {
                stacks.Insert(0, first);
                return false;
            }
            wastes.Add(new BoonSimulationItemWasted(minItem.Src, minItem.BoonDuration, minItem.Start, minItem.ApplicationTime));
            if (minItem.Extensions.Count > 0)
            {
                foreach ((AgentItem src, long value, long time) in minItem.Extensions)
                {
                    wastes.Add(new BoonSimulationItemWasted(src, value, minItem.Start, time));
                }
            }
            stacks[stacks.IndexOf(minItem)] = stackItem;
            stacks.Insert(0, first);
            Sort(log, stacks);
            return true;
        }
    }
}
