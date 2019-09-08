﻿using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;
using static GW2EIParser.Builders.JsonModels.JsonBuffData.JsonBuffStackStatus;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemIntensity : BuffSimulationItem
    {
        private readonly List<BuffSimulationItemDuration> _stacks = new List<BuffSimulationItemDuration>();

        public BuffSimulationItemIntensity(List<BoonStackItem> stacks) : base(stacks[0].Start, 0)
        {
            foreach (BoonStackItem stack in stacks)
            {
                var bstack = new BuffSimulationItemDuration(stack);
                _stacks.Add(bstack);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override void OverrideEnd(long end)
        {
            foreach (BuffSimulationItemDuration stack in _stacks)
            {
                stack.OverrideEnd(end);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override int GetTickingStacksCount()
        {
            return _stacks.Count;
        }

        /*public override void SetBoonDistributionItem(BuffDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log)
        {
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                item.SetBoonDistributionItem(distribs, start, end, boonid, log);
            }
        }*/


        public override List<JsonBuffStackStatusSources> GetStackStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            var res = new List<JsonBuffStackStatusSources>();
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                var subRes = new JsonBuffStackStatusSources { Src = GetActorID(item.Src, log, description)};
                if (item.IsExtension)
                {
                    subRes.SeedSrc = GetActorID(item.SeedSrc, log, description);
                }
                res.Add(subRes);
            }
            return res;
        }
    }
}
