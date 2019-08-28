using System.Collections.Generic;
using GW2EIParser.Parser;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class HealingLogic : QueueLogic
    {

        private struct CompareHealing
        {
            private static uint GetHealing(BoonStackItem stack)
            {
                return stack.SeedSrc.Healing;
            }

            public int Compare(BoonStackItem x, BoonStackItem y)
            {
                return -GetHealing(x).CompareTo(GetHealing(y));
            }
        }

        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            var comparator = new CompareHealing();
            stacks.Sort(comparator.Compare);
        }
    }
}
