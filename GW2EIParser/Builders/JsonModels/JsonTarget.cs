﻿using System.Collections.Generic;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonTarget : JsonActor
    {
        /// <summary>
        /// Game ID of the target
        /// </summary>
        public ushort Id;
        /// <summary>
        /// Total health of the target
        /// </summary>
        public int TotalHealth;
        /// <summary>
        /// Final health of the target
        /// </summary>
        public int FinalHealth;
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HealthPercentBurned;
        /// <summary>
        /// Time at which target became active
        /// </summary>
        public int FirstAware;
        /// <summary>
        /// Time at which target became inactive 
        /// </summary>
        public int LastAware;
        /// <summary>
        /// Array of double[2] that represents the health status of the target \n
        /// Value[i][0] will be the time, value[i][1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public List<double[]> HealthPercents;
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonTargetBuffs"/>
        public List<JsonTargetBuffs> Buffs;
    }
}
