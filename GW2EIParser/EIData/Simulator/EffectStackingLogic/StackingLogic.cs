using GW2EIParser.Parser;
using System.Collections.Generic;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        public abstract void Sort(ParsedLog log, List<BoonStackItem> stacks);
    }
}
