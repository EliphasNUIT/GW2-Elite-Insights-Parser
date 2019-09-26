using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class BuffSimulatorIDDuration : BuffSimulatorID
    {
        // Constructor
        public BuffSimulatorIDDuration(ParsedLog log) : base(log)
        {
        }

        public override void Activate(uint stackID)
        {
            BuffStackItem toSwap = BuffStack.Find(x => x.StackID == stackID);
            if (toSwap == null)
            {
                throw new InvalidOperationException("Activate has failed");
            }
            BuffStack.Remove(toSwap);
            BuffStack.Insert(0, toSwap);
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0 && timePassed > 0)
            {
                var toAdd = new BuffSimulationItemQueue(BuffStack);
                if (toAdd.End > toAdd.Start + timePassed)
                {
                    toAdd.OverrideEnd(toAdd.Start + timePassed);
                }
                GenerationSimulation.Add(toAdd);
                long timeDiff = BuffStack[0].BoonDuration - timePassed;
                long diff;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = BuffStack[0].BoonDuration;
                    leftOver = timePassed - diff;
                }
                else
                {
                    diff = timePassed;
                }
                BuffStack[0] = new BuffStackItemID((BuffStackItemID)BuffStack[0], diff, diff);
                for (int i = 1; i < BuffStack.Count; i++)
                {
                    BuffStack[i] = new BuffStackItemID((BuffStackItemID)BuffStack[i], diff, 0);
                }
                if (BuffStack[0].BoonDuration > 0)
                {
                    Update(leftOver);
                } 
                else if (leftOver > 0)
                {
                    for (int i = 1; i < BuffStack.Count; i++)
                    {
                        BuffStack[i] = new BuffStackItemID((BuffStackItemID)BuffStack[i], leftOver, 0);
                    }
                }
            } 
        }
    }
}
