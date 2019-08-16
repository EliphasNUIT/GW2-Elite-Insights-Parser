﻿using System.Collections.Generic;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonPlayer : JsonActor
    {      
        /// <summary>
        /// Account name of the player
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Group of the player
        /// </summary>
        public int Group { get; set; }
        /// <summary>
        /// Profession of the player
        /// </summary>
        public string Profession { get; set; }
        /// <summary>
        /// Weapons of the player \n
        /// 0-1 are the first land set, 1-2 are the second land set \n
        /// 3-4 are the first aquatic set, 5-6 are the second aquatic set \n
        /// When unknown, 'Unknown' value will appear \n
        /// If 2 handed weapon even indices will have "2Hand" as value
        /// </summary>
        public string[] Weapons { get; set; }
        /// <summary>
        /// Damage modifiers against all
        /// </summary>
        /// <seealso cref="JsonBuffDamageModifierData"/>
        public List<JsonBuffDamageModifierData> DamageModifiers { get; set; }
        /// <summary>
        /// Damage modifiers against targets \n
        /// Length == # of targets
        /// </summary>
        /// <seealso cref="JsonBuffDamageModifierData"/>
        public List<JsonBuffDamageModifierData>[] DamageModifiersTarget { get; set; }
        /// <summary>
        /// List of buff status on uptimes + states \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsUptime> BuffUptimes { get; set; }
        /// <summary>
        /// List of buff status on self generation  \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> SelfBuffs { get; set; }
        /// <summary>
        /// List of buff status on group generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> GroupBuffs { get; set; }
        /// <summary>
        /// List of buff status on off group generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> OffGroupBuffs { get; set; }
        /// <summary>
        /// List of buff status on squad generation \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> SquadBuffs { get; set; }
        /// <summary>
        /// List of buff status on uptimes + states on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsUptime> BuffUptimesActive { get; set; }
        /// <summary>
        /// List of buff status on self generation on active time  \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> SelfBuffsActive { get; set; }
        /// <summary>
        /// List of buff status on group generation on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> GroupBuffsActive { get; set; }
        /// <summary>
        /// List of buff status on off group generation on active time \n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> OffGroupBuffsActive { get; set; }
        /// <summary>
        /// List of buff status on squad generation on active time\n
        /// Key is "'b' + id"
        /// </summary>
        /// <seealso cref="JsonPlayerBuffsUptime"/>
        public List<JsonPlayerBuffsGeneration> SquadBuffsActive { get; set; }
        /// <summary>
        /// List of death recaps \n
        /// Length == number of death
        /// </summary>
        /// <seealso cref="JsonDeathRecap"/>
        public List<JsonDeathRecap> DeathRecap { get; set; }
        /// <summary>
        /// List of used consumables
        /// </summary>
        /// <seealso cref="JsonConsumable"/>
        public List<JsonConsumable> Consumables { get; set; }
        /// <summary>
        /// List of time during which the player was active (not dead and not dc) \n
        /// Length == number of phases
        /// </summary>
        public List<long> ActiveTimes { get; set; }
    }
}
