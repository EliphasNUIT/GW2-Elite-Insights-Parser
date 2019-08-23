using System;
using System.Collections.Generic;
using System.Text;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonCombatReplayActors
    {
        public abstract class JsonAbstractMasterActorCombatReplay
        {
            public string Img { get; set; }
            public int ID { get; set; }
            public double[] Positions { get; set; }
        }

        public class JsonMobCombatReplay : JsonAbstractMasterActorCombatReplay
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

        public class JsonPlayerCombatReplay : JsonAbstractMasterActorCombatReplay
        {
            public int Group { get; set; }
            public long[] Dead { get; set; }
            public long[] Down { get; set; }
            public long[] Dc { get; set; }
        }


        public class JsonTargetCombatReplay : JsonAbstractMasterActorCombatReplay
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

    }
}
