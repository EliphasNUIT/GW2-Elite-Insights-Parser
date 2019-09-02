﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;
using static GW2EIParser.EIData.Buff;
using static GW2EIParser.Models.DefenseStatistics;
using static GW2EIParser.Models.DPSStatistics;
using static GW2EIParser.Models.GameplayStatistics;
using static GW2EIParser.Models.SupportStatistics;

namespace GW2EIParser.EIData
{
    public abstract class AbstractSingleActor : AbstractActor
    {
        // Boons
        protected HashSet<Buff> TrackedBuffs { get; } = new HashSet<Buff>();
        protected Dictionary<long, BuffsGraphModel> BuffPoints { get; set; }
        private readonly List<BuffDistributionDictionary> _boonDistribution = new List<BuffDistributionDictionary>();
        private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        // Statistics
        private readonly Dictionary<AbstractSingleActor, List<FinalDPS>> _dpsTarget = new Dictionary<AbstractSingleActor, List<FinalDPS>>();
        private List<FinalDPS> _dpsAll;
        private readonly Dictionary<AbstractSingleActor, List<FinalGameplay>> _statsTarget = new Dictionary<AbstractSingleActor, List<FinalGameplay>>();
        private List<FinalGameplayAll> _statsAll;
        private readonly Dictionary<AbstractSingleActor, List<FinalDefense>> _defensesTarget = new Dictionary<AbstractSingleActor, List<FinalDefense>>();
        private List<FinalDefenseAll> _defensesAll;
        private readonly Dictionary<AbstractSingleActor, List<FinalSupport>> _supportTarget = new Dictionary<AbstractSingleActor, List<FinalSupport>>();
        private List<FinalSupportAll> _support;
        private Dictionary<long, List<AbstractBuffEvent>> _buffsPerId;
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
                DamageLogs.AddRange(log.CombatData.GetDamageData(AgentItem).Where(x => x.IFF != ParseEnum.EvtcIFF.Friend));
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
        public List<AbstractDamageEvent> GetJustActorDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
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
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return BuffPoints;
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
                if (!c.IsBoonSimulatorCompliant(log.FightData.FightDuration))
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
                    pair.Value.Add(new BuffRemoveAllEvent(GeneralHelper.UnknownAgent, AgentItem, dsp.Time, int.MaxValue, log.SkillData.Get(pair.Key), 1, int.MaxValue));
                }
            }
            buffMap.Sort();
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in buffMap)
            {
                TrackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
            }
            return buffMap;
        }
        public BuffDistributionDictionary GetBuffDistribution(ParsedLog log, int phaseIndex)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _boonDistribution[phaseIndex];
        }

        public Dictionary<long, long> GetBuffPresence(ParsedLog log, int phaseIndex)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _buffPresence[phaseIndex];
        }

        protected void SetBuffStatus(ParsedLog log)
        {
            BuffPoints = new Dictionary<long, BuffsGraphModel>();
            BuffDictionary toUse = GetBuffMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfBoonsID]);
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfConditionsID]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Condition].Select(x => x.ID));
            List<PhaseData> phases = log.FightData.GetPhases(log);
            // Init data
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BuffDistributionDictionary());
                _buffPresence.Add(new Dictionary<long, long>());
            }
            foreach (Buff buff in TrackedBuffs)
            {
                long boonid = buff.ID;
                if (toUse.TryGetValue(boonid, out List<AbstractBuffEvent> logs) && logs.Count != 0)
                {
                    if (BuffPoints.ContainsKey(boonid))
                    {
                        continue;
                    }
                    BuffSimulator simulator = buff.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    simulator.Trim(dur);
                    bool updateBoonPresence = boonIds.Contains(boonid);
                    bool updateCondiPresence = condiIds.Contains(boonid);
                    var graphSegments = new List<BuffsGraphModel.SegmentWithSources>();
                    // Uptime + generation
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            Add(_buffPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                            simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
                        }
                        BuffsGraphModel.SegmentWithSources segment = simul.ToSegment();
                        if (graphSegments.Count == 0 && segment.Start > 0)
                        {
                            graphSegments.Add(new BuffsGraphModel.SegmentWithSources(0, segment.Start, 0, GeneralHelper.UnknownAgent));
                        }
                        else if (graphSegments.Last().End != segment.Start)
                        {
                            graphSegments.Add(new BuffsGraphModel.SegmentWithSources(graphSegments.Last().End, segment.Start, 0, GeneralHelper.UnknownAgent));
                        }
                        graphSegments.Add(segment);
                    }
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new BuffsGraphModel.SegmentWithSources(graphSegments.Last().End, dur, 0, GeneralHelper.UnknownAgent));
                    }
                    else
                    {
                        graphSegments.Add(new BuffsGraphModel.SegmentWithSources(0, dur, 0, GeneralHelper.UnknownAgent));
                    }
                    // Wasted and Overstack
                    var extraSimulations = new List<AbstractBuffSimulationItem>(simulator.OverstackSimulationResult);
                    extraSimulations.AddRange(simulator.WasteSimulationResult);
                    foreach (AbstractBuffSimulationItem simul in extraSimulations)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
                        }
                    }
                    BuffPoints[boonid] = new BuffsGraphModel(buff, graphSegments);
                    // Condition/Boon graphs
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        List<BuffsGraphModel.Segment> segmentsToFill = updateBoonPresence ? boonPresenceGraph.BoonChart : condiPresenceGraph.BoonChart;
                        bool firstPass = segmentsToFill.Count == 0;
                        foreach (BuffsGraphModel.Segment seg in BuffPoints[boonid].BoonChart)
                        {
                            long start = seg.Start;
                            long end = seg.End;
                            int value = seg.Value > 0 ? 1 : 0;
                            if (firstPass)
                            {
                                segmentsToFill.Add(new BuffsGraphModel.Segment(start, end, value));
                            }
                            else
                            {
                                for (int i = 0; i < segmentsToFill.Count; i++)
                                {
                                    BuffsGraphModel.Segment curSeg = segmentsToFill[i];
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
                                        segmentsToFill.Insert(i + 1, new BuffsGraphModel.Segment(start, end, curVal + value));
                                        segmentsToFill.Insert(i + 2, new BuffsGraphModel.Segment(end, curEnd, curVal));
                                        break;
                                    }
                                    else
                                    {
                                        curSeg.End = start;
                                        segmentsToFill.Insert(i + 1, new BuffsGraphModel.Segment(start, curEnd, curVal + value));
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
            BuffPoints[ProfHelper.NumberOfBoonsID] = boonPresenceGraph;
            BuffPoints[ProfHelper.NumberOfConditionsID] = condiPresenceGraph;
        }
        // DPS

        public List<FinalDPS> GetDPS(ParsedLog log)
        {
            if (_dpsAll == null)
            {
                _dpsAll = GetFinalDPS(this, log, null);
            }
            return _dpsAll;
        }

        public List<FinalDPS> GetDPS(ParsedLog log, AbstractSingleActor target)
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
        public List<FinalGameplayAll> GetStats(ParsedLog log)
        {
            if (_statsAll == null)
            {
                _statsAll = GetFinalGameplay(this, log);
            }
            return _statsAll;
        }

        public List<FinalGameplay> GetStats(ParsedLog log, AbstractSingleActor target)
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
        public List<FinalDefenseAll> GetDefenses(ParsedLog log)
        {
            if (_defensesAll == null)
            {
                _defensesAll = GetFinalDefenses(this, log);
            }
            return _defensesAll;
        }

        public List<FinalDefense> GetDefenses(ParsedLog log, AbstractSingleActor target)
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
        public List<FinalSupportAll> GetSupport(ParsedLog log)
        {
            if (_support == null)
            {
                if (_buffsPerId == null)
                {
                    _buffsPerId = log.CombatData.GetBuffDataBySrcNoExt(AgentItem).GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
                }
                _support = GetFinalSupport(this, log, _buffsPerId);
            }
            return _support;
        }
        public List<FinalSupport> GetSupport(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_supportTarget.ContainsKey(target))
            {
                if (_buffsPerId == null)
                {
                    _buffsPerId = log.CombatData.GetBuffDataBySrcNoExt(AgentItem).GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
                }
                _supportTarget[target] = GetFinalSupport(log, target, _buffsPerId);
            }
            return _supportTarget[target];
        }

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
