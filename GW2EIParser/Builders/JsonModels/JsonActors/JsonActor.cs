using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Base class for Players and Targets
    /// </summary>
    /// <seealso cref="JsonPlayer"/> 
    /// <seealso cref="JsonNPC"/>
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

        protected JsonActor(AbstractActor actor)
        {
            // Meta data
            Name = actor.Character;
            Condition = actor.Condition;
            Concentration = actor.Concentration;
            Healing = actor.Healing;
            Toughness = actor.Toughness;
            HitboxHeight = actor.HitboxHeight;
            HitboxWidth = actor.HitboxWidth;
        }
    }
}
