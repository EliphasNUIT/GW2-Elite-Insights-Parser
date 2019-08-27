﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Builders.JsonModels.JsonMechanics;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// The root of the JSON
    /// </summary>
    public class JsonLog
    {

        public class Desc
        {
            /// <summary>
            /// Name of the item
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Icon of the item
            /// </summary>
            public string Icon { get; set; }
        }

        /// <summary>
        /// Describes the skill item
        /// </summary>
        public class SkillDesc : Desc
        {
            public SkillDesc(SkillItem item)
            {
                Name = item.Name;
                AutoAttack = item.AA;
                Icon = item.Icon;
            }

            /// <summary>
            /// If the skill is an auto attack
            /// </summary>
            public bool AutoAttack { get; set; }
        }

        /// <summary>
        /// Describs the buff item
        /// </summary>
        public class BuffDesc : Desc
        {
            public BuffDesc(Buff item)
            {
                Name = item.Name;
                Icon = item.Icon;
                Stacking = item.Type == Buff.BuffType.Intensity;
                Nature = item.Nature;
                Source = item.Source;
            }
            /// <summary>
            /// True if the buff is stacking
            /// </summary>
            public bool Stacking { get; set; }
            /// <summary>
            /// Nature of the buff \n
            /// <seealso cref="Buff.BuffNature"/>
            /// </summary>
            [DefaultValue(null)]
            public Buff.BuffNature Nature { get; set; }
            /// <summary>
            /// Nature of the buff \n
            /// <seealso cref="Buff.BuffSource"/>
            /// </summary>
            [DefaultValue(null)]
            public Buff.BuffSource Source { get; set; }
        }

        /// <summary>
        /// Describs the damage modifier item
        /// </summary>
        public class DamageModDesc : Desc
        {
            public DamageModDesc(DamageModifier item)
            {
                Name = item.Name;
                Icon = item.Icon;
                Description = item.Tooltip;
                NonMultiplier = !item.Multiplier;
            }
            /// <summary>
            /// Description of the damage modifier
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// False if the modifier is multiplicative \n
            /// If true then the correspond <see cref="JsonBuffDamageModifierData.JsonBuffDamageModifierItem.DamageGain"/> are damage done under the effect. One will have to deduce the gain manualy depending on your gear.
            /// </summary>
            public bool NonMultiplier { get; set; }
        }

        /// <summary>
        /// The used EI version
        /// </summary>
        public string EliteInsightsVersion { get; set; }
        /// <summary>
        /// The id with which the log has been triggered
        /// </summary>
        public int TriggerID { get; set; }
        /// <summary>
        /// The name of the fight
        /// </summary>
        public string FightName { get; set; }
        /// <summary>
        /// The icon of the fight
        /// </summary>
        public string FightIcon { get; set; }
        /// <summary>
        /// The used arcdps version
        /// </summary>
        public string ArcVersion { get; set; }
        /// <summary>
        /// The player who recorded the fight
        /// </summary>
        public string RecordedBy { get; set; }
        /// <summary>
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStart { get; set; }
        /// <summary>
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEnd { get; set; }
        /// <summary>
        /// The duration of the fight in "xh xm xs xms" format
        /// </summary>
        public string Duration { get; set; }
        /// <summary>
        /// The success status of the fight
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// The list of enemies
        /// </summary>
        /// <seealso cref="JsonActor"/>
        public List<JsonActor> Enemies { get; set; }
        /// <summary>
        /// The list of friendlies
        /// </summary>
        /// <seealso cref="JsonActor"/>
        public List<JsonActor> Friendlies { get; set; }
        /// <summary>
        /// The list of phases
        /// </summary>
        /// <seealso cref="JsonPhase"/>
        public List<JsonPhase> Phases { get; set; }
        /// <summary>
        /// List of mechanics
        /// </summary>
        /// <seealso cref="JsonMechanics"/>
        public List<JsonMechanics> Mechanics { get; set; }
        /// <summary>
        /// Upload links to dps.reports/raidar
        /// </summary>
        public List<string> UploadLinks { get; set; }
        /// <summary>
        /// Dictionary of descriptions
        /// </summary>
        /// <seealso cref="SkillDesc"/>
        /// <seealso cref="BuffDesc"/>
        public Dictionary<string, Desc> Descriptions { get; set; } = new Dictionary<string, Desc>();
        /// <summary>
        /// Dictionary of personal buffs. The key is the profession, the value is a list of buff ids
        /// </summary>
        public Dictionary<string, HashSet<string>> PersonalBuffs { get; set; } = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// Combat Replay data
        /// </summary>
        /// <seealso cref="JsonCombatReplay"/>
        public JsonCombatReplay CombatReplayData { get; set; }


        public JsonLog(ParsedLog log, string[] uploadLink)
        {
            // Meta data
            TriggerID = log.FightData.ID;
            FightName = log.FightData.Name;
            FightIcon = log.FightData.Logic.Icon;
            EliteInsightsVersion = Application.ProductVersion;
            ArcVersion = log.LogData.BuildVersion;
            RecordedBy = log.LogData.PoVName;
            TimeStart = log.LogData.LogStart;
            TimeEnd = log.LogData.LogEnd;
            Duration = log.FightData.DurationString;
            Success = log.FightData.Success;
            // Phases
            Phases = log.FightData.GetPhases(log).Select(x => new JsonPhase(log, x)).ToList();
            // Mechanics
            Mechanics = BuildMechanics(log);
            // Players
            var friendlies = new List<AbstractSingleActor>();
            var enemies = new List<AbstractSingleActor>();
            foreach (NPC npc in log.FightData.Logic.NPCs)
            {
                if (npc.Friendly)
                {
                    friendlies.Add(npc);
                }
                else
                {
                    enemies.Add(npc);
                }
            }
            // Friendlies
            Friendlies = new List<JsonActor>();
            foreach (Player p in log.PlayerList)
            {
                Friendlies.Add(new JsonPlayer(log, p, Descriptions, enemies, friendlies));
            }
            // Targets
            Enemies = new List<JsonActor>();
            foreach (NPC tar in log.FightData.Logic.NPCs)
            {
                if (tar.Friendly)
                {
                    Friendlies.Add(new JsonNPC(log, tar, Descriptions, enemies, friendlies));
                }
                else
                {
                    Enemies.Add(new JsonNPC(log, tar, Descriptions, friendlies, enemies));
                }
            }
            //
            if (uploadLink.FirstOrDefault(x => x.Length > 0) != null)
            {
                UploadLinks = uploadLink.ToList();
            }
            if (Properties.Settings.Default.ParseCombatReplay && log.CanCombatReplay)
            {
                CombatReplayData = new JsonCombatReplay(log);
            }
        }
    }
}
