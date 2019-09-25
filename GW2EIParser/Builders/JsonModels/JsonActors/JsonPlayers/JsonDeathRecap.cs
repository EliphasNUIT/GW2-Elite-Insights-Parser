using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.EIData.Player;
using static GW2EIParser.EIData.Player.DeathRecap;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to a death recap
    /// </summary>
    public class JsonDeathRecap
    {
        /// <summary>
        /// Elementary death recap item
        /// </summary>
        public class JsonDeathRecapDamageItem
        {
            /// <summary>
            /// Id of the skill
            /// </summary>
            /// <seealso cref="JsonLog.Descriptions"/>
            public string Id { get; set; }
            /// <summary>
            /// Source of the damage
            /// </summary>
            public string Src { get; set; }
            /// <summary>
            /// Damage done
            /// </summary>
            public int Damage { get; set; }
            /// <summary>
            /// Time value
            /// </summary>
            [DefaultValue(null)]
            public int Time { get; set; }

            public JsonDeathRecapDamageItem(DeathRecapDamageItem item, ParsedLog log, Dictionary<string, Desc> description)
            {
                Id = (item.IndirectDamage ? "b" : "s") + item.ID;
                Src = GetActorID(item.Src, log, description);
                Damage = item.Damage;
                Time = item.Time;
            }
        }

        /// <summary>
        /// Time of death
        /// </summary>
        [DefaultValue(null)]
        public int DeathTime { get; set; }
        /// <summary>
        /// List of damaging events to put into downstate
        /// </summary>
        public List<JsonDeathRecapDamageItem> ToDown { get; set; }
        /// <summary>
        /// List of damaging events to put into deadstate
        /// </summary>
        public List<JsonDeathRecapDamageItem> ToKill { get; set; }

        public JsonDeathRecap(DeathRecap recap, ParsedLog log, Dictionary<string, Desc> description)
        {
            DeathTime = recap.DeathTime;
            ToDown = recap.ToDown?.Select(x => new JsonDeathRecapDamageItem(x, log, description)).ToList();
            ToKill = recap.ToKill?.Select(x => new JsonDeathRecapDamageItem(x, log, description)).ToList();
        }

    }
}
