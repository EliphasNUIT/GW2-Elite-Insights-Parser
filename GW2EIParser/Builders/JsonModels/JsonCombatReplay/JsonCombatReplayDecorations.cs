using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2EIParser.Builders.JsonModels
{
    public static class JsonCombatReplayDecorations
    {
        public class JsonCombatReplayGenericDecoration
        {
            public string Type { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
            public object ConnectedTo { get; set; }
        }
        public class JsonCombatReplayFormDecoration : JsonCombatReplayGenericDecoration
        {
            public bool Fill { get; set; }
            public int Growing { get; set; }
            public string Color { get; set; }
        }

        public class JsonCombatReplayCircleDecoration : JsonCombatReplayFormDecoration
        {
            public int Radius { get; set; }
            public int MinRadius { get; set; }
        }

        public class JsonCombatReplayDoughnutDecoration : JsonCombatReplayFormDecoration
        {
            public int InnerRadius { get; set; }
            public int OuterRadius { get; set; }
        }


        public class JsonCombatReplayFacingDecoration : JsonCombatReplayGenericDecoration
        {
            public List<int> FacingData { get; set; }
        }


        public class JsonCombatReplayFacingRectangleDecoration : JsonCombatReplayFacingDecoration
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string Color { get; set; }
        }


        public class JsonCombatReplayRectangleDecoration : JsonCombatReplayFormDecoration
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }

        public class JsonCombatReplayRotatedRectangleDecoration : JsonCombatReplayRectangleDecoration
        {
            public int Rotation { get; set; }
            public int RadialTranslation { get; set; }
            public int SpinAngle { get; set; }
        }

        public class JsonCombatReplayPieDecoration : JsonCombatReplayCircleDecoration
        {
            public int Direction { get; set; }
            public int OpeningAngle { get; set; }
        }

        public class JsonCombatReplayBackgroundDecoration : JsonCombatReplayGenericDecoration
        {
        }
        public class PositionConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var positions = (List<(double x, double y, double z, double angle, double opacity, int time)>)value;
                writer.WriteStartArray();
                foreach ((double x, double y, double z, double angle, double opacity, int time) position in positions)
                {
                    (double x, double y, double z, double angle, double opacity, int time) = position;
                    writer.WriteStartArray();
                    writer.WriteValue(x);
                    writer.WriteValue(y);
                    writer.WriteValue(z);
                    writer.WriteValue(angle);
                    writer.WriteValue(opacity);
                    writer.WriteValue(time);
                    writer.WriteEndArray();
                }

                writer.WriteEndArray();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof((double x, double y, double z, double angle, double opacity, int time));
            }
        }

        public class JsonCombatReplayMovingPlatformDecoration : JsonCombatReplayBackgroundDecoration
        {
            public string Image { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }

            [JsonConverter(typeof(PositionConverter))]
            public List<(double x, double y, double z, double angle, double opacity, int time)> Positions { get; set; }
        }

        public class JsonCombatReplayLineDecoration : JsonCombatReplayFormDecoration
        {
            public object ConnectedFrom { get; set; }
        }
    }
}
