using System.Collections.Generic;
using System.ComponentModel;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonMinion : JsonSingleActor
    {
        /// <summary>
        /// Time at which minion became active
        /// </summary>
        [DefaultValue(null)]
        public int FirstAware { get; set; }
        /// <summary>
        /// Time at which minion became inactive 
        /// </summary>
        [DefaultValue(null)]
        public int LastAware { get; set; }

        public JsonMinion(ParsedLog log, Minion minion, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets, IEnumerable<AbstractMasterActor> allies) : base(log, minion, skillMap, buffMap, targets, allies)
        {
            // meta data
            FirstAware = (int)(log.FightData.ToFightSpace(minion.FirstAwareLogTime));
            LastAware = (int)(log.FightData.ToFightSpace(minion.LastAwareLogTime));
            Condition = 0;
            Concentration = 0;
            Toughness = 0;
            Healing = 0;
            HitboxHeight = 0;
            HitboxWidth = 0;
        }
    }
}
