using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;

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
            /// <summary>
            /// Time a which the event happened
            /// </summary>
            public long Time { get; set; }
            /// <summary>
            /// The actor who was hit by the mechanic
            /// </summary>
            public string Actor { get; set; }
        }

        /// <summary>
        /// List of mechanics application
        /// </summary>
        public List<JsonMechanic> MechanicsData { get; set; }
        /// <summary>
        /// Name of the mechanic
        /// </summary>
        public string Name { get; set; }


        public static List<JsonMechanics> ComputeMechanics(ParsedLog log)
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
            List<JsonMechanics> res = new List<JsonMechanics>();
            Dictionary<string, List<JsonMechanic>> dict = new Dictionary<string, List<JsonMechanic>>();
            foreach (MechanicEvent ml in mechanicLogs)
            {
                JsonMechanic mech = new JsonMechanic
                {
                    Time = ml.Time,
                    Actor = ml.Actor.Character
                };
                if (dict.TryGetValue(ml.InGameName, out var list))
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
            foreach (var pair in dict)
            {
                res.Add(new JsonMechanics()
                {
                    Name = pair.Key,
                    MechanicsData = pair.Value
                });
            }
            return res;
        }
    }
}
