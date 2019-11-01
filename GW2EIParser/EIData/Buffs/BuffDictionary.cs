using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData.CombatEvents;

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
                return x.CompareTo(y);
            }
        }


        public void Sort()
        {
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in this)
            {
                pair.Value.Sort(CompareApplicationType);
            }
        }

    }

}

