using System.Collections.Generic;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Base class for Players and Targets
    /// </summary>
    /// <seealso cref="JsonPlayer"/> 
    /// <seealso cref="JsonTarget"/>
    public abstract class JsonActor
    {

        /// <summary>
        /// Name of the actor
        /// </summary>
        public string Name { get; set; }
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
        /// List of minions
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public List<JsonMinions> Minions { get; set; }
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageDist { get; set; }
        /// <summary>
        /// Damage taken array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageTaken { get; set; }
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation { get; set; }
        /// <summary>
        /// Array of int[2] that represents the number of conditions status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of conditions present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public List<int[]> ConditionsStates { get; set; }
        /// <summary>
        /// Array of int[2] that represents the number of boons status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of boons present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> BoonsStates { get; set; }
        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatsAll"/>
        public JsonStatsAll[] StatsAll { get; set; }
        /// <summary>
        /// Stats against targets  \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonStats"/>
        public JsonStats[][] StatsTargets { get; set; }
        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefenses"/>
        public JsonDefenses[] Defenses { get; set; }
        /// <summary>
        /// Support stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonSupport"/>
        public JsonSupport[] Support { get; set; }
        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonDPS[] DpsAll { get; set; }
        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonDPS[][] DpsTargets { get; set; }
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[][] TargetDamageDist { get; set; }
    }
}
