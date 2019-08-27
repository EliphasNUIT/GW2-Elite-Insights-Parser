using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonNPC : JsonMasterActor
    {
        /// <summary>
        /// Game ID of the target
        /// </summary>
        public ushort Id { get; set; }
        /// <summary>
        /// Total health of the target
        /// </summary>
        public int TotalHealth { get; set; }
        /// <summary>
        /// Final health of the target
        /// </summary>
        public int FinalHealth { get; set; }
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HealthPercentBurned { get; set; }
        /// <summary>
        /// Time at which target became active
        /// </summary>
        [DefaultValue(null)]
        public int FirstAware { get; set; }
        /// <summary>
        /// Time at which target became inactive 
        /// </summary>
        [DefaultValue(null)]
        public int LastAware { get; set; }
        /// <summary>
        /// Array of double[2] that represents the health status of the target \n
        /// Value[i][0] will be the time, value[i][1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public List<double[]> HealthPercents { get; set; }
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonNPCBuffs"/>
        public List<JsonNPCBuffs> Buffs { get; set; }

        public JsonNPC(ParsedLog log, NPC target, Dictionary<string, Desc> description, IEnumerable<AbstractSingleActor> targets, IEnumerable<AbstractSingleActor> allies) : base(log, target, description, targets, allies)
        {
            // meta data
            Id = target.ID;
            TotalHealth = target.GetHealth(log.CombatData);
            FirstAware = (int)(log.FightData.ToFightSpace(target.FirstAwareLogTime));
            LastAware = (int)(log.FightData.ToFightSpace(target.LastAwareLogTime));
            double hpLeft = 0.0;
            List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(target.AgentItem);
            if (log.FightData.Success)
            {
                hpLeft = 0;
            }
            else
            {
                if (hpUpdates.Count > 0)
                {
                    hpLeft = hpUpdates.Last().HPPercent;
                }
            }
            HealthPercents = hpUpdates.Select(x => new double[2] { x.Time, x.HPPercent }).ToList();
            HealthPercentBurned = 100.0 - hpLeft;
            FinalHealth = (int)Math.Round(TotalHealth * hpLeft / 100.0);
        }
    }
}
