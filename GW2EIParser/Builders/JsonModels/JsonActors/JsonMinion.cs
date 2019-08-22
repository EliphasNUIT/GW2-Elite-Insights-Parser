using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonMinion : JsonSingleActor
    {
        /// <summary>
        /// Time at which minion became active
        /// </summary>
        public int FirstAware { get; set; }
        /// <summary>
        /// Time at which minion became inactive 
        /// </summary>
        public int LastAware { get; set; }

        public JsonMinion(ParsedLog log, Minion minion, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets) : base(log, minion, skillMap, buffMap, targets)
        {
            // meta data
            FirstAware = (int)(log.FightData.ToFightSpace(minion.FirstAwareLogTime));
            LastAware = (int)(log.FightData.ToFightSpace(minion.LastAwareLogTime));
        }
    }
}
