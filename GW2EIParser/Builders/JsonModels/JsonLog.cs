using System.Collections.Generic;
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
            /// <summary>
            /// Type of the description
            /// </summary>
            [DefaultValue(null)]
            public int Type { get; set; }
        }

        /// <summary>
        /// Describes the skill item, type == 0
        /// </summary>
        public class SkillDesc : Desc
        {
            public SkillDesc(SkillItem item)
            {
                Name = item.Name;
                AutoAttack = item.AA;
                Icon = item.Icon;
                Type = 0;
                CanCrit = item.CanCrit;
            }

            /// <summary>
            /// If the skill is an auto attack
            /// </summary>
            public bool AutoAttack { get; set; }
            /// <summary>
            /// If the skill is can do a critical hit
            /// </summary>
            public bool CanCrit { get; set; }
        }

        /// <summary>
        /// Describs the buff item, type == 1
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
                Type = 1;
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
        /// Describs the damage modifier item, type == 2
        /// </summary>
        public class DamageModDesc : Desc
        {
            public DamageModDesc(DamageModifier item)
            {
                Name = item.Name;
                Icon = item.Icon;
                Description = item.Tooltip;
                NonMultiplier = !item.Multiplier;
                Type = 2;
            }
            /// <summary>
            /// Description of the damage modifier
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// False if the modifier is multiplicative \n
            /// If true then the correspond <see cref="JsonDamageModifierData.JsonBuffDamageModifierItem.DamageGain"/> are damage done under the effect. One will have to deduce the gain manualy depending on your gear.
            /// </summary>
            public bool NonMultiplier { get; set; }
        }

        /// <summary>
        /// Simple icon description, for ui stuff, type == 3
        /// </summary>
        public class IconDesc : Desc
        {
            public IconDesc()
            {
                Type = 3;
            }
        }

        /// <summary>
        /// Agent description
        /// </summary>
        public abstract class AgentDesc : Desc
        {
            /// <summary>
            /// Master of this agent
            /// </summary>
            public string MasterID { get; set; }
            /// <summary>
            /// Condition damage score
            /// </summary>
            public uint Condition { get; set; }
            /// <summary>
            /// Concentration score
            /// </summary>
            public uint Concentration { get; set; }
            /// <summary>
            /// Healing Power score
            /// </summary>
            public uint Healing { get; set; }
            /// <summary>
            /// Toughness score
            /// </summary>
            public uint Toughness { get; set; }
            /// <summary>
            /// Height of the hitbox
            /// </summary>
            public uint HitboxHeight { get; set; }
            /// <summary>
            /// Width of the hitbox
            /// </summary>
            public uint HitboxWidth { get; set; }
            /// <summary>
            /// Time at which actor became active
            /// </summary>
            [DefaultValue(null)]
            public int FirstAware { get; set; }
            /// <summary>
            /// Time at which actor became inactive 
            /// </summary>
            [DefaultValue(null)]
            public int LastAware { get; set; }
            public AgentDesc(AbstractSingleActor actor, ParsedLog log)
            {
                Name = actor.Character;
                Icon = actor.Icon;
                if (actor.AgentItem.MasterAgent != null)
                {
                    MasterID = actor.AgentItem.MasterAgent.UniqueID;
                }
                Condition = actor.Condition;
                Concentration = actor.Concentration;
                Healing = actor.Healing;
                Toughness = actor.Toughness;
                HitboxHeight = actor.HitboxHeight;
                HitboxWidth = actor.HitboxWidth;
                FirstAware = (int)(log.FightData.ToFightSpace(actor.FirstAwareLogTime));
                LastAware = (int)(log.FightData.ToFightSpace(actor.LastAwareLogTime));
            }
        }

        /// <summary>
        /// Player description, type == 4
        /// </summary>
        public class PlayerDesc : AgentDesc
        {
            /// <summary>
            /// Account name of the player
            /// </summary>
            public string Account { get; set; }
            /// <summary>
            /// Group of the player
            /// </summary>
            [DefaultValue(null)]
            public int Group { get; set; }
            /// <summary>
            /// Profession of the player
            /// </summary>
            public string Profession { get; set; }
            public PlayerDesc(Player player, ParsedLog log) : base(player, log)
            {

                Account = player.Account;
                Group = player.Group;
                Profession = player.Prof;
                Type = 4;
            }
        }

        /// <summary>
        /// NPC description, type == 5
        /// </summary>
        public class NPCDesc : AgentDesc
        {
            /// <summary>
            /// Game ID of the npc
            /// </summary>
            public ushort Id { get; set; }
            /// <summary>
            /// Total health of the npc
            /// </summary>
            public int TotalHealth { get; set; }
            public NPCDesc(NPC npc, ParsedLog log) : base(npc, log)
            {
                Id = npc.ID;
                TotalHealth = npc.GetHealth(log.CombatData);
                Type = 5;
            }
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

        public static string GetActorID(AgentItem ag, ParsedLog log, Dictionary<string, Desc> description)
        {
            string res = ag.UniqueID;
            // Players are always in description
            string descID = ag.Type == AgentItem.AgentType.EnemyPlayer ? res : "npc" + ag.ID;
            // Find actor will return null if agent is not present in the known agent pool
            if (log.FindActor(ag, true, false) == null)
            {
                res = descID;
                // special case for WvW
                if (!description.ContainsKey(descID))
                {
                    // create a dummy npc for the description
                    var dummyNPC = new NPC(ag, false);
                    description[descID] = new NPCDesc(dummyNPC, log);
                }
            }
            return res;
        }

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
            //
            Descriptions["toughness"] = new IconDesc { Icon = "https://wiki.guildwars2.com/images/1/12/Toughness.png" };
            Descriptions["condition"] = new IconDesc { Icon = "https://wiki.guildwars2.com/images/5/54/Condition_Damage.png" };
            Descriptions["concentration"] = new IconDesc { Icon = "https://wiki.guildwars2.com/images/5/54/Condition_Damage.png" };
            Descriptions["healingPower"] = new IconDesc { Icon = "https://wiki.guildwars2.com/images/8/81/Healing_Power.png" };
            // Friendlies
            Friendlies = new List<JsonActor>();
            foreach (Player p in log.PlayerList)
            {
                Friendlies.Add(new JsonPlayer(log, p, Descriptions));
            }
            // Targets
            Enemies = new List<JsonActor>();
            foreach (NPC tar in log.FightData.Logic.NPCs)
            {
                if (tar.Friendly)
                {
                    Friendlies.Add(new JsonNPC(log, tar, Descriptions));
                }
                else
                {
                    Enemies.Add(new JsonNPC(log, tar, Descriptions));
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
