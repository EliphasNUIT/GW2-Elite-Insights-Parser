using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to the regrouping of the same type of minions
    /// </summary>
    public class JsonMinions
    {
        public List<JsonNPC> MinionList { get; set; }
        public JsonMinions(ParsedLog log, Minions minions, Dictionary<string, Desc> description)
        {
            if (!minions.MinionList.Any())
            {
                return;
            }
            MinionList = minions.MinionList.Select(x => new JsonNPC(log, x, description)).ToList();
        }
    }
}
