using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemQueue : BuffSimulationItemDuration
    {

        private readonly List<BuffSimulationItemDuration> _queue = new List<BuffSimulationItemDuration>();

        public BuffSimulationItemQueue(List<BoonStackItem> queue) : base(queue.First())
        {
            for (int i = 1; i < queue.Count; i++)
            {
                var stack = new BuffSimulationItemDuration(queue[i]);
                _queue.Add(stack);
            }
        }

        public override List<object> GetStackStatusList()
        {
            var res = new List<object>()
            {
                new object[]{Src.UniqueID}
            };
            foreach (BuffSimulationItemDuration item in _queue)
            {
                res.Add(new object[] { item.Src.UniqueID, item.Duration });
            }
            return res;
        }
    }
}
