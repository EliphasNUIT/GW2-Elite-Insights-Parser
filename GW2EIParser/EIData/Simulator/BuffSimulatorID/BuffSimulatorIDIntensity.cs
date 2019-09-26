using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class BuffSimulatorIDIntensity : BuffSimulatorID
    {
        // Constructor
        public BuffSimulatorIDIntensity(ParsedLog log) : base(log)
        {
        }

        public override void Activate(uint stackID)
        {
            // nothing to do, all stack are active
            //throw new InvalidOperationException("Activate on intensity buff??");
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0 && timePassed > 0)
            {
                var toAdd = new BuffSimulationItemIntensity(BuffStack); 
                if (toAdd.End > toAdd.Start + timePassed)
                {
                    toAdd.OverrideEnd(toAdd.Start + timePassed);
                }
                GenerationSimulation.Add(toAdd);
                long diff = Math.Min(BuffStack.Min(x => x.BoonDuration), timePassed);
                long leftOver = timePassed - diff;
                // Subtract from each
                for (int i = BuffStack.Count - 1; i >= 0; i--)
                {
                    var item = new BuffStackItemID((BuffStackItemID)BuffStack[i], diff, diff);
                    BuffStack[i] = item;
                }
                if (BuffStack.Any(x => x.BoonDuration == 0) && leftOver > 0)
                {
                    for (int i = BuffStack.Count - 1; i >= 0; i--)
                    {
                        var item = new BuffStackItemID((BuffStackItemID)BuffStack[i], leftOver, leftOver);
                        BuffStack[i] = item;
                    }
                    return;
                } 
                else
                {
                    Update(leftOver);
                }
            }
        }
    }
}
