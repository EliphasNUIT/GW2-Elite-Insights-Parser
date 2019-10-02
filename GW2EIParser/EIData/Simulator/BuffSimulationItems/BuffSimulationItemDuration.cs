using System;
using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Builders.JsonModels.JsonBuffData.JsonBuffStackStatus;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.EIData.AbstractBuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemDuration : BuffSimulationItem
    {
        public AgentItem Src { get; }
        public AgentItem SeedSrc { get; }
        public bool IsExtension { get; }
        public long ID { get; private set; }

        public BuffSimulationItemDuration(BuffStackItem other) : base(other.Start, other.Duration)
        {
            Src = other.Src;
            SeedSrc = other.SeedSrc;
            IsExtension = other.IsExtension;
            ID = other.ID;
        }

        public override void OverrideEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetTickingStacksCount()
        {
            return 1;
        }

        /*public override void SetBoonDistributionItem(BuffDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = GetDistrib(distribs, boonid);
            long cDur = GetClampedDuration(start, end);
            AgentItem agent = Src;
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
        }*/

        public override List<JsonBuffStackStatusSources> GetStackStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            throw new InvalidOperationException();
        }
    }
}
