using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.EIData.Player;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonPlayer : JsonSingleActor
    {
        /// <summary>
        /// Weapons of the player \n
        /// 0-1 are the first land set, 1-2 are the second land set \n
        /// 3-4 are the first aquatic set, 5-6 are the second aquatic set \n
        /// When unknown, 'Unknown' value will appear \n
        /// If 2 handed weapon even indices will have "2Hand" as value
        /// </summary>
        public List<string> Weapons { get; set; }
        /// <summary>
        /// List of death recaps \n
        /// Length == number of death
        /// </summary>
        /// <seealso cref="JsonDeathRecap"/>
        public List<JsonDeathRecap> DeathRecaps { get; set; }
        /// <summary>
        /// List of used consumables
        /// </summary>
        /// <seealso cref="JsonConsumable"/>
        public List<JsonConsumable> Consumables { get; set; }
        /// <summary>
        /// Statistics data
        /// </summary>
        /// <seealso cref="JsonStatistics"/>
        //public JsonStatistics Statistics { get; set; }


        public JsonPlayer(ParsedLog log, Player player, Dictionary<string, Desc> description) : base(log, player, description)
        {
            // meta data
            Weapons = player.GetWeaponsArray(log).Select(w => w ?? "").ToList();
            foreach (string wep in Weapons)
            {
                if (wep.Length > 0 && !description.ContainsKey(wep))
                {
                    description.Add(wep, new IconDesc() { Icon = GeneralHelper.GetWeaponIcon(wep) });
                }
            }
            // Death Recap
            List<DeathRecap> deathRecaps = player.GetDeathRecaps(log);
            DeathRecaps = deathRecaps?.Select(x => new JsonDeathRecap(x, log, description)).ToList();
            // Consumables
            Consumables = JsonConsumable.BuildConsumables(player.GetConsumablesList(log), description);
            // Stats
            //Statistics = new JsonStatistics(log, player);
            //
            DescriptionID = UniqueID;
            //
            if (!description.ContainsKey(UniqueID))
            {
                description.Add(UniqueID, new PlayerDesc(player, log));
            }
        }
    }
}
