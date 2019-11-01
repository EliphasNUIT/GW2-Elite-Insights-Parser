using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GW2EIParser.Logic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class Player : AbstractSingleActor
    {
        public class DamageModifierData
        {
            public int HitCount { get; }
            public int TotalHitCount { get; }
            public double DamageGain { get; }
            public int TotalDamage { get; }

            public DamageModifierData(int hitCount, int totalHitCount, double damageGain, int totalDamage)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                TotalDamage = totalDamage;
            }
        }

        public class Consumable
        {
            public Buff Buff { get; }
            public long Time { get; }
            public int Duration { get; }
            public int Stack { get; set; }
            public bool Initial { get; set; }

            public Consumable(Buff item, long time, int duration, bool initial)
            {
                Buff = item;
                Time = time;
                Duration = duration;
                Stack = 1;
                Initial = initial;
            }
        }

        public class DeathRecap
        {
            public class DeathRecapDamageItem
            {
                public long ID { get; set; }
                public bool IndirectDamage { get; set; }
                public AgentItem Src { get; set; }
                public int Damage { get; set; }
                public int Time { get; set; }
            }

            public int DeathTime { get; set; }
            public List<DeathRecapDamageItem> ToDown { get; set; }
            public List<DeathRecapDamageItem> ToKill { get; set; }
        }
        // Fields
        public string Account { get; protected set; }
        public int Group { get; }

        private List<Consumable> _consumeList;
        private List<DeathRecap> _deathRecaps;
        private Dictionary<string, List<DamageModifierData>> _damageModifiers;
        private HashSet<string> _presentDamageModifiers;
        private Dictionary<NPC, Dictionary<string, List<DamageModifierData>>> _damageModifiersTargets;
        //weaponslist
        private string[] _weaponsArray;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            Friendly = agent.Type == AgentItem.AgentType.Player;
            string[] name = agent.Name.Split('\0');
            if (name.Length < 2)
            {
                throw new InvalidOperationException("Name problem on Player");
            }
            if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains("-"))
            {
                throw new InvalidOperationException("Missing Group on Player");
            }
            Account = name[1].TrimStart(':');
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            IsFakeActor = Account == "Conjured Sword";
        }

        public void Anonymize(int index)
        {
            Character = "Player " + index;
            Account = "Account " + index;
        }
        // Death Recap
        public List<DeathRecap> GetDeathRecaps(ParsedLog log)
        {
            if (_deathRecaps == null)
            {
                SetDeathRecaps(log);
            }
            if (_deathRecaps.Count == 0)
            {
                return null;
            }
            return _deathRecaps;
        }

        private void SetDeathRecaps(ParsedLog log)
        {
            _deathRecaps = new List<DeathRecap>();
            List<DeathRecap> res = _deathRecaps;
            List<DeadEvent> deads = log.CombatData.GetDeadEvents(AgentItem);
            List<DownEvent> downs = log.CombatData.GetDownEvents(AgentItem);
            long lastDeathTime = 0;
            List<AbstractDamageEvent> damageLogs = GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration);
            foreach (DeadEvent dead in deads)
            {
                var recap = new DeathRecap()
                {
                    DeathTime = (int)dead.Time
                };
                DownEvent downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastDeathTime);
                if (downed != null)
                {
                    var damageToDown = damageLogs.Where(x => x.Time <= downed.Time && (x.HasHit || x.HasDowned) && x.Time > lastDeathTime).ToList();
                    recap.ToDown = damageToDown.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToDown.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToDown[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag
                        };
                        damage += dl.Damage;
                        recap.ToDown.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                    var damageToKill = damageLogs.Where(x => x.Time > downed.Time && x.Time <= dead.Time && (x.HasHit || x.HasDowned) && x.Time > lastDeathTime).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToKill[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag
                        };
                        recap.ToKill.Add(item);
                    }
                }
                else
                {
                    recap.ToDown = null;
                    var damageToKill = damageLogs.Where(x => x.Time < dead.Time && x.Damage > 0 && x.Time > lastDeathTime).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToKill[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag
                        };
                        damage += dl.Damage;
                        recap.ToKill.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                }
                lastDeathTime = dead.Time;
                res.Add(recap);
            }
        }
        // Weapons
        public string[] GetWeaponsArray(ParsedLog log)
        {
            if (_weaponsArray == null)
            {
                EstimateWeapons(log);
            }
            return _weaponsArray;
        }

        private void EstimateWeapons(ParsedLog log)
        {
            if (Prof == "Sword")
            {
                _weaponsArray = new string[]
                {
                    "Sword",
                    "2Hand",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                };
                return;
            }
            string[] weapons = new string[8];//first 2 for first set next 2 for second set, second sets of 4 for underwater
            List<AbstractCastEvent> casting = GetCastLogs(log, 0, log.FightData.FightDuration);
            int swapped = -1;
            long swappedTime = 0;
            var swaps = casting.Where(x => x.SkillId == SkillItem.WeaponSwapId).Select(x =>
            {
                if (x is WeaponSwapEvent wse)
                {
                    return wse.SwappedTo;
                }
                return -1;
            }).ToList();
            foreach (AbstractCastEvent cl in casting)
            {
                if (cl.ActualDuration == 0 && cl.SkillId != SkillItem.WeaponSwapId)
                {
                    continue;
                }
                SkillItem skill = cl.Skill;
                // first iteration
                if (swapped == -1)
                {
                    swapped = skill.FindWeaponSlot(swaps);
                }
                if (!skill.EstimateWeapons(weapons, swapped, cl.Time > swappedTime) && cl is WeaponSwapEvent swe)
                {
                    //wepswap  
                    swapped = swe.SwappedTo;
                    swappedTime = swe.Time;
                }
            }
            _weaponsArray = weapons;
        }
        // Consumables
        public List<Consumable> GetConsumablesList(ParsedLog log)
        {
            if (_consumeList == null)
            {
                SetConsumablesList(log);
            }
            return _consumeList;
        }

        private void SetConsumablesList(ParsedLog log)
        {
            List<Buff> consumableList = log.Buffs.BuffsByNature[BuffNature.Consumable];
            _consumeList = new List<Consumable>();
            long fightDuration = log.FightData.FightDuration;
            foreach (Buff consumable in consumableList)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(consumable.ID))
                {
                    if (!(c is BuffApplyEvent ba) || AgentItem != ba.To)
                    {
                        continue;
                    }
                    long time = ba.Time;
                    if (time <= fightDuration)
                    {
                        Consumable existing = _consumeList.Find(x => x.Time == time && x.Buff.ID == consumable.ID);
                        if (existing != null)
                        {
                            existing.Stack++;
                        }
                        else
                        {
                            _consumeList.Add(new Consumable(consumable, time, ba.AppliedDuration, ba.Initial));
                        }
                    }
                }
            }
            _consumeList.Sort((x, y) => x.Time.CompareTo(y.Time));

        }
        // Damage modifiers
        public Dictionary<string, List<DamageModifierData>> GetDamageModifierData(ParsedLog log, NPC target)
        {
            if (_damageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            if (target != null)
            {
                if (_damageModifiersTargets.TryGetValue(target, out Dictionary<string, List<DamageModifierData>> res))
                {
                    return res;
                }
                else
                {
                    return new Dictionary<string, List<DamageModifierData>>();
                }
            }
            return _damageModifiers;
        }

        public HashSet<string> GetPresentDamageModifier(ParsedLog log)
        {
            if (_presentDamageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            return _presentDamageModifiers;
        }

        private void SetDamageModifiersData(ParsedLog log)
        {
            _damageModifiers = new Dictionary<string, List<DamageModifierData>>();
            _damageModifiersTargets = new Dictionary<NPC, Dictionary<string, List<DamageModifierData>>>();
            _presentDamageModifiers = new HashSet<string>();
            // If conjured sword or WvW, stop
            if (IsFakeActor || log.FightData.Logic.Mode == FightLogic.ParseMode.WvW)
            {
                return;
            }
            /*var damageMods = new List<DamageModifier>(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.ItemBuff]);
            damageMods.AddRange(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.CommonBuff]);
            damageMods.AddRange(log.DamageModifiers.GetModifiersPerProf(Prof));
            foreach (DamageModifier mod in damageMods)
            {
                mod.ComputeDamageModifier(_damageModifiers, _damageModifiersTargets, this, log);
            }
            _presentDamageModifiers.UnionWith(_damageModifiers.Keys);
            foreach (NPC tar in _damageModifiersTargets.Keys)
            {
                _presentDamageModifiers.UnionWith(_damageModifiersTargets[tar].Keys);
            }*/
        }
        // Combat Replay
        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            if (IsFakeActor)
            {
                return;
            }
            // Fight related stuff
            log.FightData.Logic.ComputePlayerCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }


        public override JsonAbstractSingleActorCombatReplay GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            (List<(long start, long end)> deads, List<(long start, long end)> downs, List<(long start, long end)> dcs) = GetStatus(log);
            var aux = new JsonPlayerCombatReplay
            {
                Group = Group,
                ID = AgentItem.UniqueID,
                Positions = new List<double>(),
                Dead = new List<long>(),
                Down = new List<long>(),
                Dc = new List<long>()
            };
            foreach (Point3D pos in CombatReplay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions.Add(x);
                aux.Positions.Add(y);
            }
            foreach ((long start, long end) in deads)
            {
                aux.Dead.Add(start);
                aux.Dead.Add(end);
            }
            foreach ((long start, long end) in downs)
            {
                aux.Down.Add(start);
                aux.Down.Add(end);
            }
            foreach ((long start, long end) in dcs)
            {
                aux.Dc.Add(start);
                aux.Dc.Add(end);
            }

            return aux;
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CombatData.HasMovementData || IsFakeActor)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay();
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightDuration, true);
        }
    }
}

