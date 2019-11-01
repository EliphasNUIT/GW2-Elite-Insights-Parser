using System.Collections.Generic;
using GW2EIParser.Parser;
using static GW2EIParser.EIData.AbstractBuffSimulator;

namespace GW2EIParser.EIData
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffOverrideItem> overrides);

        public abstract void Sort(ParsedLog log, List<BuffStackItem> stacks);
    }
}
