using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class Minions : AbstractActor
    {
        public int MinionID { get; }
        public List<NPC> MinionList { get; }
        private List<AbstractDamageEvent> _damageLogs;
        private Dictionary<AgentItem, List<AbstractDamageEvent>> _damageLogsByDst;
        private List<AbstractDamageEvent> _damageTakenLogs;
        private Dictionary<AgentItem, List<AbstractDamageEvent>> _damageTakenLogsByDst;
        private List<AbstractCastEvent> _castLogs;

        public Minions(NPC firstMinion) : base(firstMinion.AgentItem)
        {
            MinionList = new List<NPC> { firstMinion };
        }

        public void AddMinion(NPC minion)
        {
            MinionList.Add(minion);
        }
        // Damage logs
        public override List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (_damageLogs == null)
            {
                _damageLogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in MinionList)
                {
                    _damageLogs.AddRange(minion.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
                }
                _damageLogsByDst = _damageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && _damageLogsByDst.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return _damageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        public override List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (_damageTakenLogs == null)
            {
                _damageTakenLogs = new List<AbstractDamageEvent>();
                foreach (NPC minion in MinionList)
                {
                    _damageTakenLogs.AddRange(minion.GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration));
                }
                _damageTakenLogsByDst = _damageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null && _damageTakenLogsByDst.TryGetValue(target.AgentItem, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return _damageTakenLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        // Cast logs
        public override List<AbstractCastEvent> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (_castLogs == null)
            {
                _castLogs = new List<AbstractCastEvent>();
                foreach (NPC minion in MinionList)
                {
                    _castLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightDuration));
                }
                _castLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
            return _castLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

    }
}
