using System;
using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemDuration : BuffSimulationItem
    {
        private readonly AgentItem _src;
        private readonly AgentItem _seedSrc;
        private readonly bool _isExtension;

        public BuffSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            _src = other.Src;
            _seedSrc = other.SeedSrc;
            _isExtension = other.IsExtension;
            Sources = new List<AgentItem>() { _src };
        }

        public override void OverrideEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetTickingStacksCount()
        {
            return 1;
        }
        public override int GetStacksCount()
        {
            return 1;
        }

        public override void SetBoonDistributionItem(BuffDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = GetDistrib(distribs, boonid);
            long cDur = GetClampedDuration(start, end);
            AgentItem agent = _src;
            AgentItem seedAgent = _seedSrc;
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.Generation += cDur;
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    cDur,
                    0, 0, 0, 0, 0));
            }
            if (_isExtension)
            {
                if (distrib.TryGetValue(agent, out toModify))
                {
                    toModify.ByExtension += cDur;
                    distrib[agent] = toModify;
                }
                else
                {
                    distrib.Add(agent, new BuffDistributionItem(
                        0,
                        0, 0, 0, cDur, 0));
                }
            }
            if (agent != seedAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.Extended += cDur;
                    distrib[seedAgent] = toModify;
                }
                else
                {
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }
            if (agent == GeneralHelper.UnknownAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.UnknownExtended += cDur;
                    distrib[seedAgent] = toModify;
                }
                else
                {
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
