using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class LineDecoration : FormDecoration
    {
        public Connector ConnectedFrom { get; }
        public int Width { get; }

        public LineDecoration(int growing, (int start, int end) lifespan, string color, Connector connector, Connector targetConnector) : base(false, growing, lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
        }


        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            JsonCombatReplayLineDecoration aux = new JsonCombatReplayLineDecoration
            {
                Type = "Line",
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log),
                ConnectedFrom = ConnectedFrom.GetConnectedTo(map, log)
            };
            return aux;
        }
    }
}
