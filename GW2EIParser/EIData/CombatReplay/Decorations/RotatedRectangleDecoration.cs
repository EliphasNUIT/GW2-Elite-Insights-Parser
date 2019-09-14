using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class RotatedRectangleDecoration : RectangleDecoration
    {
        public int Rotation { get; } // initial rotation angle
        public int RadialTranslation { get; } // translation of the triangle center in the direction of the current rotation
        public int SpinAngle { get; } // rotation the rectangle is supposed to go through over the course of its lifespan, 0 for no rotation

        // Rectangles with fixed rotation and no translation
        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, int rotation, (int start, int end) lifespan, string color, Connector connector)
            : this(fill, growing, width, height, rotation, 0, 0, lifespan, color, connector) { }


        // Rectangles with a fixed rotation and translation

        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, int rotation, int translation, (int start, int end) lifespan, string color, Connector connector)
            : this(fill, growing, width, height, rotation, translation, 0, lifespan, color, connector) { }

        // Rectangles rotating over time

        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, int rotation, int translation, int spinAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, width, height, lifespan, color, connector)
        {
            Rotation = rotation;
            RadialTranslation = translation;
            SpinAngle = spinAngle;
        }

        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            var aux = new JsonCombatReplayRotatedRectangleDecoration
            {
                Type = "RotatedRectangle",
                Width = Width,
                Height = Height,
                Rotation = Rotation,
                RadialTranslation = RadialTranslation,
                SpinAngle = SpinAngle,
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
