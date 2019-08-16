using GW2EIParser.EIData;
using System.Collections.Generic;

namespace GW2EIParser.Builders.HtmlModels
{
    public class BoonDto
    {    
        public long Id;       
        public string Name;       
        public string Icon;       
        public bool Stacking;
        public bool Consumable;
        public bool Enemy;

        public static void AssembleBoons(ICollection<Buff> boons, Dictionary<string, BoonDto> dict)
        {
            foreach (Buff boon in boons)
            {
                dict["b" + boon.ID] = new BoonDto()
                {
                    Id = boon.ID,
                    Name = boon.Name,
                    Icon = boon.Link,
                    Stacking = (boon.Type == Buff.BoonType.Intensity),
                    Consumable = (boon.Nature == Buff.BoonNature.Consumable),
                    Enemy = (boon.Source == Buff.BoonSource.Enemy)
                };
            }
        }
    }
}
