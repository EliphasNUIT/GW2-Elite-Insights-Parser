﻿using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class DoughnutDecoration : FormDecoration
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutDecoration(bool fill, int growing, int innerRadius, int outerRadius, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        //

        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            var aux = new JsonCombatReplayDoughnutDecoration
            {
                Type = "Doughnut",
                OuterRadius = OuterRadius,
                InnerRadius = InnerRadius,
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
