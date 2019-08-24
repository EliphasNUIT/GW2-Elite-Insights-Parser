using System.Collections.Generic;
using GW2EIParser.EIData;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public static class BuffStatistics
    {

        public enum BuffEnum { Self, Group, OffGroup, Squad };

        public class FinalTargetBuffs
        {
            public FinalTargetBuffs(List<Player> plist)
            {
                Uptime = 0;
                Presence = 0;
                Generated = new Dictionary<Player, double>();
                Overstacked = new Dictionary<Player, double>();
                Wasted = new Dictionary<Player, double>();
                UnknownExtension = new Dictionary<Player, double>();
                Extension = new Dictionary<Player, double>();
                Extended = new Dictionary<Player, double>();
                foreach (Player p in plist)
                {
                    Generated.Add(p, 0);
                    Overstacked.Add(p, 0);
                    Wasted.Add(p, 0);
                    UnknownExtension.Add(p, 0);
                    Extension.Add(p, 0);
                    Extended.Add(p, 0);
                }
            }

            public double Uptime { get; set; }
            public double Presence { get; set; }
            public Dictionary<Player, double> Generated { get; }
            public Dictionary<Player, double> Overstacked { get; }
            public Dictionary<Player, double> Wasted { get; }
            public Dictionary<Player, double> UnknownExtension { get; }
            public Dictionary<Player, double> Extension { get; }
            public Dictionary<Player, double> Extended { get; }
        }
        public class FinalBuffs
        {
            public double Uptime { get; set; }
            public double Generation { get; set; }
            public double Overstack { get; set; }
            public double Wasted { get; set; }
            public double UnknownExtended { get; set; }
            public double ByExtension { get; set; }
            public double Extended { get; set; }
            public double Presence { get; set; }
        }

    }
}
