using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public class MovingPlatformDecoration : BackgroundDecoration
    {
        protected string Image { get; }
        protected int Width { get; }
        protected int Height { get; }

        protected List<(double x, double y, double z, double angle, double opacity, int time)> Positions { get; } =
            new List<(double x, double y, double z, double angle, double opacity, int time)>();

        public MovingPlatformDecoration(string image, int width, int height, (int start, int end) lifespan) : base(lifespan)
        {
            Image = image;
            Width = width;
            Height = height;
        }


        public void AddPosition(double x, double y, double z, double angle, double opacity, int time)
        {
            Positions.Add((x, y, z, angle, opacity, time));
        }

        public override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            var positions = Positions.OrderBy(x => x.time).Select(pos =>
            {
                (double mapX, double mapY) = map.GetMapCoord((float)pos.x, (float)pos.y);
                pos.x = mapX;
                pos.y = mapY;

                return pos;
            }).ToList();

            var aux = new JsonCombatReplayMovingPlatformDecoration
            {
                Type = "MovingPlatform",
                Image = Image,
                Width = Width,
                Height = Height,
                Start = Lifespan.start,
                End = Lifespan.end,
                Positions = positions
            };
            return aux;
        }
    }
}