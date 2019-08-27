using System.Collections.Generic;

namespace GW2EIParser.Builders.JsonModels
{
    public static class JsonCombatReplayActors
    {
        public abstract class JsonAbstractSingleActorCombatReplay
        {
            public string Img { get; set; }
            public int ID { get; set; }
            public int MasterID { get; set; }
            public List<double> Positions { get; set; }
        }

        public class JsonMobCombatReplay : JsonAbstractSingleActorCombatReplay
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

        public class JsonPlayerCombatReplay : JsonAbstractSingleActorCombatReplay
        {
            public int Group { get; set; }
            public List<long> Dead { get; set; }
            public List<long> Down { get; set; }
            public List<long> Dc { get; set; }
        }


        public class JsonTargetCombatReplay : JsonAbstractSingleActorCombatReplay
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

    }
}
