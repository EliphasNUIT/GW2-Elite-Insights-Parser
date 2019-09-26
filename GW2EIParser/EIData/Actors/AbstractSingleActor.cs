using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;
//using static GW2EIParser.Builders.JsonModels.JsonStatistics;
using static GW2EIParser.EIData.Buff;
/*using static GW2EIParser.Models.DefenseStatistics;
using static GW2EIParser.Models.DPSStatistics;
using static GW2EIParser.Models.GameplayStatistics;
using static GW2EIParser.Models.SupportStatistics;*/

namespace GW2EIParser.EIData
{
    public abstract class AbstractSingleActor : AbstractActor
    {
        // Boons
        protected HashSet<Buff> TrackedBuffs { get; } = new HashSet<Buff>();
        private Dictionary<long, BuffsGraphModel> _buffPoints;
        //private readonly List<BuffDistributionDictionary> _boonDistribution = new List<BuffDistributionDictionary>();
        //private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        // Statistics
        /*private readonly Dictionary<AbstractSingleActor, List<JsonDPS>> _dpsTarget = new Dictionary<AbstractSingleActor, List<JsonDPS>>();
        private List<JsonDPS> _dpsAll;
        private readonly Dictionary<AbstractSingleActor, List<JsonGameplay>> _statsTarget = new Dictionary<AbstractSingleActor, List<JsonGameplay>>();
        private List<JsonGameplayAll> _statsAll;
        private readonly Dictionary<AbstractSingleActor, List<JsonDefense>> _defensesTarget = new Dictionary<AbstractSingleActor, List<JsonDefense>>();
        private List<JsonDefenseAll> _defensesAll;
        private readonly Dictionary<AbstractSingleActor, List<JsonSupport>> _supportTarget = new Dictionary<AbstractSingleActor, List<JsonSupport>>();
        private List<JsonSupportAll> _support;*/
        //status
        private List<(long start, long end)> _deads;
        private List<(long start, long end)> _downs;
        private List<(long start, long end)> _dCs;
        // Minions
        private Dictionary<long, Minions> _minions;
        // Replay
        protected CombatReplay CombatReplay { get; set; }
        // Friendly
        public bool Friendly { get; protected set; }

        public string Icon { get; }

        protected AbstractSingleActor(AgentItem agent) : base(agent)
        {
            Icon = GeneralHelper.GetIcon(this);
        }
        // Status

        public (List<(long start, long end)>, List<(long start, long end)>, List<(long start, long end)>) GetStatus(ParsedLog log)
        {
            if (_deads == null)
            {
                _deads = new List<(long start, long end)>();
                _downs = new List<(long start, long end)>();
                _dCs = new List<(long start, long end)>();
                AgentItem.GetAgentStatus(_deads, _downs, _dCs, log);
            }
            return (_deads, _downs, _dCs);
        }

        // Damage logs
        public override List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractDamageEvent>();
                DamageLogs.AddRange(log.CombatData.GetDamageData(AgentItem).Where(x => x.IFF != ParseEnum.IFF.Friend));
                Dictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    DamageLogs.AddRange(mins.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
                }
                DamageLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
                DamageLogsByDst = DamageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageLogsByDst.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractDamageEvent>();
                }
            }
            return DamageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageTakenlogs == null)
            {
                DamageTakenlogs = new List<AbstractDamageEvent>();
                DamageTakenlogs.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
                DamageTakenLogsBySrc = DamageTakenlogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenLogsBySrc.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
                {
                    long targetStart = log.FightData.ToFightSpace(target.FirstAwareLogTime);
                    long targetEnd = log.FightData.ToFightSpace(target.LastAwareLogTime);
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractDamageEvent>();
                }
            }
            return DamageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        public override List<AbstractDamageEvent> GetJustActorDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            return GetDamageLogs(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        // Cast logs
        public override List<AbstractCastEvent> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();

        }
        protected void SetCastLogs(ParsedLog log)
        {
            CastLogs = new List<AbstractCastEvent>(log.CombatData.GetCastData(AgentItem));
            foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
            {
                if (CastLogs.Count > 0 && (wepSwap.Time - CastLogs.Last().Time) < 10 && CastLogs.Last().SkillId == SkillItem.WeaponSwapId)
                {
                    CastLogs[CastLogs.Count - 1] = wepSwap;
                }
                else
                {
                    CastLogs.Add(wepSwap);
                }
            }
            CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        // Buffs
        public Dictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedLog log)
        {
            if (_buffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _buffPoints;
        }

        protected BuffDictionary GetBuffMap(ParsedLog log)
        {
            //
            var buffMap = new BuffDictionary();
            // Fill in Boon Map
            foreach (AbstractBuffEvent c in log.CombatData.GetBuffDataByDst(AgentItem))
            {
                long buffId = c.BuffID;
                if (!buffMap.ContainsKey(buffId))
                {
                    if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
                    {
                        continue;
                    }
                    buffMap.Add(log.Buffs.BuffsByIds[buffId]);
                }
                if (!c.IsBuffSimulatorCompliant(log.FightData.FightDuration, log.CombatData.HasStackIDs))
                {
                    continue;
                }
                List<AbstractBuffEvent> loglist = buffMap[buffId];
                c.TryFindSrc(log);
                loglist.Add(c);
            }
            // add buff remove all for each despawn events
            foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(AgentItem))
            {
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in buffMap)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(GeneralHelper.UnknownAgent, AgentItem, dsp.Time, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            buffMap.Sort();
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in buffMap)
            {
                TrackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
            }
            return buffMap;
        }
        /*public BuffDistributionDictionary GetBuffDistribution(ParsedLog log, int phaseIndex)
        {
            if (_buffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _boonDistribution[phaseIndex];
        }

        public Dictionary<long, long> GetBuffPresence(ParsedLog log, int phaseIndex)
        {
            if (_buffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _buffPresence[phaseIndex];
        }*/

        protected void SetBuffStatus(ParsedLog log)
        {
            _buffPoints = new Dictionary<long, BuffsGraphModel>();
            BuffDictionary toUse = GetBuffMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfBoonsID]);
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfConditionsID]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Condition].Select(x => x.ID));
            List<PhaseData> phases = log.FightData.GetPhases(log);
            // Init data
            /*for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BuffDistributionDictionary());
                _buffPresence.Add(new Dictionary<long, long>());
            }*/
            foreach (Buff buff in TrackedBuffs)
            {
                long boonid = buff.ID;
                if (toUse.TryGetValue(boonid, out List<AbstractBuffEvent> logs) && logs.Count != 0)
                {
                    if (_buffPoints.ContainsKey(boonid))
                    {
                        continue;
                    }
                    AbstractBuffSimulator simulator = buff.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    simulator.Trim(dur);
                    bool updateBoonPresence = boonIds.Contains(boonid);
                    bool updateCondiPresence = condiIds.Contains(boonid);
                    var graphSegments = new List<BuffSegment>();
                    // Uptime + generation
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        /*for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            Add(_buffPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                            simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
                        }*/
                        if (graphSegments.Count > 0 && graphSegments.Last().End != simul.Start)
                        {
                            graphSegments.Add(new BuffSegment(graphSegments.Last().End, simul.Start, 0));
                        }
                        graphSegments.Add(new BuffSegment(simul.Start, simul.End, simul.GetTickingStacksCount()));
                    }
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new BuffSegment(graphSegments.Last().End, dur, 0));
                    }
                    else
                    {
                        graphSegments.Add(new BuffSegment(0, dur, 0));
                    }
                    // Wasted and Overstack
                    /*var extraSimulations = new List<AbstractBuffSimulationItem>(simulator.OverstackSimulationResult);
                    extraSimulations.AddRange(simulator.WasteSimulationResult);
                    foreach (AbstractBuffSimulationItem simul in extraSimulations)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
                        }
                    }*/
                    _buffPoints[boonid] = new BuffsGraphModel(buff, graphSegments, simulator);
                    // Condition/Boon graphs
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        List<BuffSegment> segmentsToFill = updateBoonPresence ? boonPresenceGraph.ValueBasedBoonChart : condiPresenceGraph.ValueBasedBoonChart;
                        bool firstPass = segmentsToFill.Count == 0;
                        foreach (BuffSegment seg in _buffPoints[boonid].ValueBasedBoonChart)
                        {
                            long start = seg.Start;
                            long end = seg.End;
                            int value = seg.Value > 0 ? 1 : 0;
                            if (firstPass)
                            {
                                segmentsToFill.Add(new BuffSegment(start, end, value));
                            }
                            else
                            {
                                for (int i = 0; i < segmentsToFill.Count; i++)
                                {
                                    BuffSegment curSeg = segmentsToFill[i];
                                    long curEnd = curSeg.End;
                                    long curStart = curSeg.Start;
                                    int curVal = curSeg.Value;
                                    if (curStart > end)
                                    {
                                        break;
                                    }
                                    if (curEnd < start)
                                    {
                                        continue;
                                    }
                                    if (end <= curEnd)
                                    {
                                        curSeg.End = start;
                                        segmentsToFill.Insert(i + 1, new BuffSegment(start, end, curVal + value));
                                        segmentsToFill.Insert(i + 2, new BuffSegment(end, curEnd, curVal));
                                        break;
                                    }
                                    else
                                    {
                                        curSeg.End = start;
                                        segmentsToFill.Insert(i + 1, new BuffSegment(start, curEnd, curVal + value));
                                        start = curEnd;
                                        i++;
                                    }
                                }
                            }
                        }
                        if (updateBoonPresence)
                        {
                            boonPresenceGraph.FuseSegments();
                        }
                        else
                        {
                            condiPresenceGraph.FuseSegments();
                        }
                    }

                }
            }
            _buffPoints[ProfHelper.NumberOfBoonsID] = boonPresenceGraph;
            _buffPoints[ProfHelper.NumberOfConditionsID] = condiPresenceGraph;
        }

        // DPS

        /*public List<JsonDPS> GetDPS(ParsedLog log)
        {
            if (_dpsAll == null)
            {
                _dpsAll = GetFinalDPS(this, log, null);
            }
            return _dpsAll;
        }

        public List<JsonDPS> GetDPS(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_dpsTarget.ContainsKey(target))
            {
                _dpsTarget[target] = GetFinalDPS(this, log, target);
            }
            return _dpsTarget[target];
        }
        // Stats
        public List<JsonGameplayAll> GetStats(ParsedLog log)
        {
            if (_statsAll == null)
            {
                _statsAll = GetFinalGameplay(this, log);
            }
            return _statsAll;
        }

        public List<JsonGameplay> GetStats(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_statsTarget.ContainsKey(target))
            {
                _statsTarget[target] = GetFinalGameplay(this, log, target);
            }
            return _statsTarget[target];
        }
        // Defenses
        public List<JsonDefenseAll> GetDefenses(ParsedLog log)
        {
            if (_defensesAll == null)
            {
                _defensesAll = GetFinalDefenses(this, log);
            }
            return _defensesAll;
        }

        public List<JsonDefense> GetDefenses(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_defensesTarget.ContainsKey(target))
            {
                _defensesTarget[target] = GetFinalDefenses(this, log, target);
            }
            return _defensesTarget[target];
        }
        // Support

        public List<JsonSupportAll> GetSupport(ParsedLog log)
        {
            if (_support == null)
            {
                _support = GetFinalSupport(this, log, GetBuffRemoveAllByID(log));
            }
            return _support;
        }
        public List<JsonSupport> GetSupport(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_supportTarget.ContainsKey(target))
            {
                _supportTarget[target] = GetFinalSupport(log, target, GetBuffRemoveAllByID(log));
            }
            return _supportTarget[target];
        }*/

        // Minions
        public Dictionary<long, Minions> GetMinions(ParsedLog log)
        {
            if (_minions == null)
            {
                _minions = new Dictionary<long, Minions>();
                var combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.MasterAgent == AgentItem).ToList();
                var auxMinions = new Dictionary<long, Minions>();
                foreach (AgentItem agent in combatMinion)
                {
                    long id = agent.ID;
                    if (auxMinions.TryGetValue(id, out Minions values))
                    {
                        values.AddMinion(new NPC(agent, Friendly));
                    }
                    else
                    {
                        auxMinions[id] = new Minions(new NPC(agent, Friendly));
                    }
                }
                foreach (KeyValuePair<long, Minions> pair in auxMinions)
                {
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightDuration).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightDuration).Count > 0)
                    {
                        _minions[pair.Key] = pair.Value;
                    }
                }
            }
            return _minions;
        }


        // Combat Replay
        protected void SetMovements(ParsedLog log)
        {
            foreach (AbstractMovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
            {
                movementEvent.AddPoint3D(CombatReplay);
            }
        }
        public List<int> GetCombatReplayTimes(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.Times;
        }

        public List<Point3D> GetCombatReplayPolledPositions(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.PolledPositions;
        }

        protected abstract void InitCombatReplay(ParsedLog log);

        protected void TrimCombatReplay(ParsedLog log)
        {
            DespawnEvent despawnCheck = log.CombatData.GetDespawnEvents(AgentItem).LastOrDefault();
            SpawnEvent spawnCheck = log.CombatData.GetSpawnEvents(AgentItem).LastOrDefault();
            DeadEvent deathCheck = log.CombatData.GetDeadEvents(AgentItem).LastOrDefault();
            if (deathCheck != null)
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), deathCheck.Time);
            }
            else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), despawnCheck.Time);
            }
            else
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), log.FightData.ToFightSpace(AgentItem.LastAwareLogTime));
            }
        }

        public List<GenericDecoration> GetCombatReplayActors(ParsedLog log)
        {
            if (!log.CanCombatReplay || IsFakeActor)
            {
                // no combat replay support on fight
                return null;
            }
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            if (CombatReplay.NoActors)
            {
                CombatReplay.NoActors = false;
                InitAdditionalCombatReplayData(log);
            }
            return CombatReplay.Actors;
        }
        protected abstract void InitAdditionalCombatReplayData(ParsedLog log);


        public abstract JsonAbstractSingleActorCombatReplay GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);
    }
}
