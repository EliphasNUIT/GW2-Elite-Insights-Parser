using GW2EIParser.EIData;
using GW2EIParser.Models;
using System.Collections.Generic;

namespace GW2EIParser.Builders.HtmlModels
{
    public class BoonData
    {
        public double Avg;       
        public List<List<object>> Data = new List<List<object>>();

        public BoonData(Dictionary<long, Statistics.FinalBuffs> boons, List<Buff> listToUse, double avg)
        {
            Avg = avg;
            foreach (Buff boon in listToUse)
            {
                List<object> boonVals = new List<object>();
                Data.Add(boonVals);

                if (boons.TryGetValue(boon.ID, out var uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Buff.BoonType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
            }
        }

        public BoonData(Dictionary<long, Statistics.FinalTargetBuffs> boons, List<Buff> listToUse, double avg)
        {
            Avg = avg;
            foreach (Buff boon in listToUse)
            {
                List<object> boonVals = new List<object>();
                Data.Add(boonVals);

                if (boons.TryGetValue(boon.ID, out var uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Buff.BoonType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
            }
        }

        public BoonData(Dictionary<long, Statistics.FinalTargetBuffs> boons, List<Buff> listToUse, Player player)
        {
            foreach (Buff boon in listToUse)
            {
                List<object> boonData = new List<object>();
                if (boons.TryGetValue(boon.ID, out var toUse))
                {
                    boonData.Add(toUse.Generated[player]);
                    boonData.Add(toUse.Overstacked[player]);
                    boonData.Add(toUse.Wasted[player]);
                    boonData.Add(toUse.UnknownExtension[player]);
                    boonData.Add(toUse.Extension[player]);
                    boonData.Add(toUse.Extended[player]);
                }
                Data.Add(boonData);
            }
        }

        public BoonData(List<Buff> listToUse, Dictionary<long, Statistics.FinalBuffs> uptimes)
        {
            foreach (Buff boon in listToUse)
            {
                if (uptimes.TryGetValue(boon.ID, out var uptime))
                {
                    Data.Add(new List<object>()
                        {
                            uptime.Generation,
                            uptime.Overstack,
                            uptime.Wasted,
                            uptime.UnknownExtended,
                            uptime.ByExtension,
                            uptime.Extended
                        });
                }
                else
                {
                    Data.Add(new List<object>()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0
                        });
                }
            }
        }

        public BoonData(string prof, Dictionary<string, List<Buff>> boonsBySpec, Dictionary<long, Statistics.FinalBuffs> boons)
        {
            foreach (Buff boon in boonsBySpec[prof])
            {
                List<object> boonVals = new List<object>();
                Data.Add(boonVals);
                if (boons.TryGetValue(boon.ID, out var uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Buff.BoonType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
                else
                {
                    boonVals.Add(0);
                }
            }
        }
    }
}
