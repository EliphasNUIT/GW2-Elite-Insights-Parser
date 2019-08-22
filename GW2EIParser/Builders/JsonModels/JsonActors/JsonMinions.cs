using GW2EIParser.EIData;
using GW2EIParser.Parser;
using System.Collections.Generic;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to the regrouping of the same type of minions
    /// </summary>
    public class JsonMinions : JsonActor
    {
        /// <summary>
        /// Total Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public List<int> TotalDamage { get; set; }
        /// <summary>
        /// Damage done by minions against targets \n
        /// Length == # of targets for <seealso cref="JsonLog.Friendlies"/> or # of players for <seealso cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        public List<List<int>> TotalTargetDamage { get; set; }
        /// <summary>
        /// Total Shield Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public List<int> TotalShieldDamage { get; set; }
        /// <summary>
        /// Shield Damage done by minions against targets \n
        /// Length == # of targets for <seealso cref="JsonLog.Friendlies"/> or # of players for <seealso cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        public List<List<int>> TotalTargetShieldDamage { get; set; }


        public JsonMinions(ParsedLog log, Minions minions, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets) : base(log, minions, skillMap, buffMap, targets)
        {
        }
    }
}
