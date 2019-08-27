using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class AbstractActor : DummyActor
    {
        public bool IsFakeActor { get; protected set; }
        // Damage
        protected List<AbstractDamageEvent> DamageLogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> DamageLogsByDst { get; set; }
        protected List<AbstractDamageEvent> DamageTakenlogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> DamageTakenLogsBySrc { get; set; }
        // Cast
        protected List<AbstractCastEvent> CastLogs { get; set; }

        protected AbstractActor(AgentItem agent) : base(agent)
        {
        }

        // Damage logs
        public abstract List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end);

        public abstract List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end);

        // Cast logs
        public abstract List<AbstractCastEvent> GetCastLogs(ParsedLog log, long start, long end);

        // Utilities
        protected static void Add<T>(Dictionary<T, long> dictionary, T key, long value)
        {
            if (dictionary.TryGetValue(key, out long existing))
            {
                dictionary[key] = existing + value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
