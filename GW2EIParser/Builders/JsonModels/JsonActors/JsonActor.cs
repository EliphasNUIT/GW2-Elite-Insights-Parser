using System.Collections.Generic;
using GW2EIParser.EIData;

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
        /// Damage distribution data
        /// </summary>
        /// <seealso cref="JsonDamageDistData"/>
        public JsonDamageDistData DamageDistributionData { get; set; }
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation { get; set; }
        /// <summary>
        /// Unique ID representing the actor
        /// </summary>
        public string UniqueID { get; set; }
        /// <summary>
        /// Unique ID representing the description of the actor
        /// </summary>
        public string DescriptionID { get; set; }
    }
}
