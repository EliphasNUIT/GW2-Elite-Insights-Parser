using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class CircleDecoration : FormDecoration
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleDecoration(bool fill, int growing, int radius, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
        }

        public CircleDecoration(bool fill, int growing, int radius, (int start, int end) lifespan, string color, Connector connector, int minRadius) : base(fill, growing, lifespan, color, connector)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        //

        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            var aux = new JsonCombatReplayCircleDecoration
            {
                Type = "Circle",
                Radius = Radius,
                MinRadius = MinRadius,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log)
            };
            return aux;
        }
    }
}
