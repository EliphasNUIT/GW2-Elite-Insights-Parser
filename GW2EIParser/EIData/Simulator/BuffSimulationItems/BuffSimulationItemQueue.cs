﻿using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonBuffData.JsonBuffStackStatus;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.EIData.AbstractBuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemQueue : BuffSimulationItemDuration
    {

        private readonly List<BuffSimulationItemDuration> _queue = new List<BuffSimulationItemDuration>();

        public BuffSimulationItemQueue(List<BuffStackItem> queue) : base(queue.First())
        {
            for (int i = 1; i < queue.Count; i++)
            {
                var stack = new BuffSimulationItemDuration(queue[i]);
                _queue.Add(stack);
            }
        }

        public BuffSimulationItemQueue(List<BuffStackItem> queue, BuffStackItem active) : base(active)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i] == active || queue[i].Duration <= 0)
                {
                    continue;
                }
                var stack = new BuffSimulationItemDuration(queue[i]);
                _queue.Add(stack);
            }
        }

        public override List<JsonBuffStackStatusSources> GetStackStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            var res = new List<JsonBuffStackStatusSources>
            {
                new JsonBuffStackStatusSources(this, log, description)
            };
            foreach (BuffSimulationItemDuration item in _queue)
            {
                res.Add(new JsonBuffStackStatusSources(item, log, description));
            }
            return res;
        }
    }
}

