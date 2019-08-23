using System.Collections.Generic;

namespace GW2EIParser.Builders.JsonModels
{
    public static class JsonCombatReplayActors
    {
        public abstract class JsonAbstractMasterActorCombatReplay
        {
            public string Img { get; set; }
            public int ID { get; set; }
            public List<double> Positions { get; set; }
        }

        public class JsonMobCombatReplay : JsonAbstractMasterActorCombatReplay
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

        public class JsonPlayerCombatReplay : JsonAbstractMasterActorCombatReplay
        {
            public int Group { get; set; }
            public List<long> Dead { get; set; }
            public List<long> Down { get; set; }
            public List<long> Dc { get; set; }
        }


        public class JsonTargetCombatReplay : JsonAbstractMasterActorCombatReplay
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

    }
}
