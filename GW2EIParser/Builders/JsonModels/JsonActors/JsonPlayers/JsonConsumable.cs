using GW2EIParser.Models;
using System.Collections.Generic;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class representing consumables
    /// </summary>
    public class JsonConsumable
    {
        /// <summary>
        /// Number of stacks
        /// </summary>
        public int Stack { get; set; }
        /// <summary>
        /// Duration of the consumable
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Time of application of the consumable
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// True if the encounter started with that consumable
        /// </summary>
        public bool Initial { get; set; }
        /// <summary>
        /// ID of the consumable
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }

        public JsonConsumable(Statistics.Consumable food)
        {
            Stack = food.Stack;
            Duration = food.Duration;
            Time = food.Time;
            Id = food.Buff.ID;
            Initial = food.Initial;
        }

        public static List<JsonConsumable> BuildConsumables(List<Statistics.Consumable> foods, Dictionary<string, BuffDesc> buffMap)
        {
            List<JsonConsumable> res = new List<JsonConsumable>();
            foreach (var food in foods)
            {
                if (!buffMap.ContainsKey("b" + food.Buff.ID))
                {
                    buffMap["b" + food.Buff.ID] = new BuffDesc(food.Buff);
                }
                res.Add(new JsonConsumable(food));
            }
            return res.Count > 0 ? res : null;
        }
    }
}
