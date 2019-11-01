using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class RectangleDecoration : FormDecoration
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleDecoration(bool fill, int growing, int width, int height, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Height = height;
            Width = width;
        }
        //


        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            var aux = new JsonCombatReplayRectangleDecoration
            {
                Type = "Rectangle",
                Width = Width,
                Height = Height,
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

