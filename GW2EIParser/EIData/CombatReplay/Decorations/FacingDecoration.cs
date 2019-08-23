using System.Collections.Generic;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class FacingDecoration : GenericDecoration
    {
        protected List<int> Data { get; } = new List<int>();

        public FacingDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach (Point3D facing in facings)
            {
                Data.Add(-Point3D.GetRotationFromFacing(facing));
            }
        }

        //

        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            JsonCombatReplayFacingDecoration aux = new JsonCombatReplayFacingDecoration
            {
                Type = "Facing",
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log),
                FacingData = new List<int>()
            };
            foreach (int angle in Data)
            {
                aux.FacingData.Add(angle);
            }
            return aux;
        }
    }
}
