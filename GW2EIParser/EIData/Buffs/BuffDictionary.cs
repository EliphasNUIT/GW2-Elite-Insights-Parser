using GW2EIParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;

namespace GW2EIParser.EIData
{
    public class BuffDictionary : Dictionary<long, List<AbstractBuffEvent>>
    {
        // Constructors
        public BuffDictionary()
        {
        }
        public BuffDictionary(Buff boon)
        {
            this[boon.ID] = new List<AbstractBuffEvent>();
        }

        public BuffDictionary(IEnumerable<Buff> boons)
        {
            foreach (Buff boon in boons)
            {
                this[boon.ID] = new List<AbstractBuffEvent>();
            }
        }


        public void Add(IEnumerable<Buff> boons)
        {
            foreach (Buff boon in boons)
            {
                if (ContainsKey(boon.ID))
                {
                    continue;
                }
                this[boon.ID] = new List<AbstractBuffEvent>();
            }
        }

        public void Add(Buff boon)
        {
            if (ContainsKey(boon.ID))
            {
                return;
            }
            this[boon.ID] = new List<AbstractBuffEvent>();
        }

        private int CompareApplicationType(AbstractBuffEvent x, AbstractBuffEvent y)
        {
            if (x.Time < y.Time)
            {
                return -1;
            }
            else if (x.Time > y.Time)
            {
                return 1;
            }
            else
            {
                if (x is BuffRemoveSingleEvent || x is BuffRemoveAllEvent)
                {
                    if (y is BuffRemoveSingleEvent || x is BuffRemoveAllEvent)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (y is BuffRemoveSingleEvent || x is BuffRemoveAllEvent)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }


        public void Sort()
        {
            foreach (var pair in this)
            {
                pair.Value.Sort(CompareApplicationType);
            }
        }
        
    }

}