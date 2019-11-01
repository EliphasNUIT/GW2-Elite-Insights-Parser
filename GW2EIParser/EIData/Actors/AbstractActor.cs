using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class AbstractActor
    {
        public AgentItem AgentItem { get; }
        public string Character { get; protected set; }

        public uint Toughness => AgentItem.Toughness;
        public uint Condition => AgentItem.Condition;
        public uint Concentration => AgentItem.Concentration;
        public uint Healing => AgentItem.Healing;
        public ushort InstID => AgentItem.InstID;
        public string Prof => AgentItem.Prof;
        public string UniqueID => AgentItem.UniqueID;
        public ulong Agent => AgentItem.Agent;
        public long LastAwareLogTime => AgentItem.LastAwareLogTime;
        public long FirstAwareLogTime => AgentItem.FirstAwareLogTime;
        public ushort ID => AgentItem.ID;
        public uint HitboxHeight => AgentItem.HitboxHeight;
        public uint HitboxWidth => AgentItem.HitboxWidth;
        public bool IsFakeActor { get; protected set; }
        // Damage
        protected List<AbstractDamageEvent> DamageLogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> DamageLogsByDst { get; set; }
        protected List<AbstractDamageEvent> DamageTakenlogs { get; set; }
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> DamageTakenLogsBySrc { get; set; }
        // Cast
        protected List<AbstractCastEvent> CastLogs { get; set; }

        protected AbstractActor(AgentItem agent)
        {
            string[] name = agent.Name.Split('\0');
            Character = name[0];
            AgentItem = agent;
        }

        // Damage logs
        public abstract List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end);

        public abstract List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end);

        public abstract List<AbstractDamageEvent> GetJustActorDamageLogs(AbstractActor target, ParsedLog log, long start, long end);

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

