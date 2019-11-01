using System.Collections.Generic;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class FacingRectangleDecoration : FacingDecoration
    {
        private readonly int _width;
        private readonly int _height;
        private readonly string _color;
        public FacingRectangleDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings, int width, int height, string color) : base(lifespan, connector, facings)
        {
            _width = width;
            _height = height;
            _color = color;
        }

        //

        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            JsonCombatReplayFacingDecoration aux = new JsonCombatReplayFacingRectangleDecoration
            {
                Type = "FacingRectangle",
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log),
                FacingData = new List<int>(),
                Width = _width,
                Height = _height,
                Color = _color
            };
            foreach (int angle in Data)
            {
                aux.FacingData.Add(angle);
            }
            return aux;
        }
    }
}

