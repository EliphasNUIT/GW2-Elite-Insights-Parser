using System.Collections.Generic;
using System.ComponentModel;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.EIData.Player;

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
        [DefaultValue(null)]
        public long Time { get; set; }
        /// <summary>
        /// True if the encounter started with that consumable
        /// </summary>
        public bool Initial { get; set; }
        /// <summary>
        /// ID of the consumable
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public string Id { get; set; }

        public JsonConsumable(Consumable food)
        {
            Stack = food.Stack;
            Duration = food.Duration;
            Time = food.Time;
            Id = "b" + food.Buff.ID;
            Initial = food.Initial;
        }

        public static List<JsonConsumable> BuildConsumables(List<Consumable> foods, Dictionary<string, Desc> description)
        {
            var res = new List<JsonConsumable>();
            foreach (Consumable food in foods)
            {
                if (!description.ContainsKey("b" + food.Buff.ID))
                {
                    description["b" + food.Buff.ID] = new BuffDesc(food.Buff);
                }
                res.Add(new JsonConsumable(food));
            }
            return res.Count > 0 ? res : null;
        }
    }
}
