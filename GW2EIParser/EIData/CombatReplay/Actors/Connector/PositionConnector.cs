using GW2EIParser.Parser;

namespace GW2EIParser.EIData
{
    public class PositionConnector : Connector
    {
        protected Point3D Position;

        public PositionConnector()
        {

        }

        public PositionConnector(Point3D position)
        {
            Position = position;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedLog log)
        {
            (double x, double y) = map.GetMapCoord(Position.X, Position.Y);
            return new double[2]
                       {
                        x,
                        y
                       };
        }
    }
}
