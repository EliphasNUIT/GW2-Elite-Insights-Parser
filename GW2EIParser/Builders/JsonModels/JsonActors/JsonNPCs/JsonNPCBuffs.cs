﻿using System.Collections.Generic;
using GW2EIParser.EIData;
using static GW2EIParser.Models.BuffStatistics;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class representing buff on targets
    /// </summary>
    public class JsonNPCBuffs
    {
        /// <summary>
        /// Target buff item
        /// </summary>
        public class JsonTargetBuffsData
        {
            /// <summary>
            /// Uptime of the buff
            /// </summary>
            public double Uptime { get; set; }
            /// <summary>
            /// Presence of the buff (intensity only)
            /// </summary>
            public double Presence { get; set; }
            /// <summary>
            /// Buff generated by
            /// </summary>
            public Dictionary<string, double> Generated { get; set; }
            /// <summary>
            /// Buff overstacked by
            /// </summary>
            public Dictionary<string, double> Overstacked { get; set; }
            /// <summary>
            /// Buff wasted by
            /// </summary>
            public Dictionary<string, double> Wasted { get; set; }
            /// <summary>
            /// Buff extended by unknown for
            /// </summary>
            public Dictionary<string, double> UnknownExtended { get; set; }
            /// <summary>
            /// Buff by extension
            /// </summary>
            public Dictionary<string, double> ByExtension { get; set; }
            /// <summary>
            /// Buff extended for
            /// </summary>
            public Dictionary<string, double> Extended { get; set; }


            private static Dictionary<string, double> ConvertKeys(Dictionary<Player, double> toConvert)
            {
                Dictionary<string, double> res = new Dictionary<string, double>();
                foreach (var pair in toConvert)
                {
                    res[pair.Key.Character] = pair.Value;
                }
                return res;
            }

            public JsonTargetBuffsData(FinalTargetBuffs stats)
            {
                Uptime = stats.Uptime;
                Presence = stats.Presence;
                Generated = ConvertKeys(stats.Generated);
                Overstacked = ConvertKeys(stats.Overstacked);
                Wasted = ConvertKeys(stats.Wasted);
            }
        }

        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonTargetBuffsData"/>
        public List<JsonTargetBuffsData> BuffData { get; set; }
        /// <summary>
        /// Array of int[2] that represents the number of the given buff status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of the buff present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> States { get; set; }
    }

}
