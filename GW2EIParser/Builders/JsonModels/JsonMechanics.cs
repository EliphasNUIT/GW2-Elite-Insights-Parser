using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using Newtonsoft.Json;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to mechanics
    /// </summary>
    public class JsonMechanics
    {
        /// <summary>
        /// Class corresponding to a mechanic event
        /// </summary>
        public class JsonMechanic
        {
            [JsonIgnore]
            public MechanicEvent MechEvent { get; set; }
            /// <summary>
            /// Time a which the event happened
            /// </summary>
            [DefaultValue(null)]
            public long Time { get; set; }
            /// <summary>
            /// The actor who was hit by the mechanic
            /// </summary>
            public string Actor { get; set; }

            public JsonMechanic(MechanicEvent mechEvent)
            {
                MechEvent = mechEvent;
                Time = mechEvent.Time;
                Actor = mechEvent.Actor.Character;
            }
        }

        /// <summary>
        /// List of mechanics application
        /// </summary>
        public List<JsonMechanic> MechanicsData { get; set; }
        /// <summary>
        /// Name of the mechanic
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the mechanic
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// True if the mechanic impact every party member (like failing a cc)
        /// </summary>
        public bool FightMechanic { get; set; }


        public static List<JsonMechanics> BuildMechanics(ParsedLog log)
        {
            MechanicData mechanicData = log.MechanicData;
            var mechanicLogs = new List<MechanicEvent>();
            foreach (List<MechanicEvent> mLog in mechanicData.GetAllMechanics(log))
            {
                mechanicLogs.AddRange(mLog);
            }
            if (mechanicLogs.Count == 0)
            {
                return null;
            }
            var res = new List<JsonMechanics>();
            var dict = new Dictionary<string, List<JsonMechanic>>();
            foreach (MechanicEvent ml in mechanicLogs)
            {
                var mech = new JsonMechanic(ml);
                if (dict.TryGetValue(ml.InGameName, out List<JsonMechanic> list))
                {
                    list.Add(mech);
                }
                else
                {
                    dict[ml.InGameName] = new List<JsonMechanic>()
                        {
                            mech
                        };
                }
            }
            foreach (KeyValuePair<string, List<JsonMechanic>> pair in dict)
            {
                MechanicEvent first = pair.Value.First().MechEvent;
                res.Add(new JsonMechanics()
                {
                    Name = pair.Key,
                    Description = first.Description,
                    FightMechanic = first.Enemy,
                    MechanicsData = pair.Value
                });
            }
            return res;
        }
    }
}
