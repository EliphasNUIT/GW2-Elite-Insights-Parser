using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;
using static GW2EIParser.Models.Statistics;

namespace GW2EIParser.EIData
{
    public abstract class AbstractSingleActor : AbstractActor
    {
        // Damage
        private readonly Dictionary<AbstractActor, List<AbstractDamageEvent>> _selfDamageLogsPerTarget = new Dictionary<AbstractActor, List<AbstractDamageEvent>>();
        // Boons
        public HashSet<Buff> TrackedBuffs { get; } = new HashSet<Buff>();
        protected Dictionary<long, BuffsGraphModel> BuffPoints;
        private readonly List<BuffDistribution> _boonDistribution = new List<BuffDistribution>();
        private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        // Statistics
        private Dictionary<AbstractMasterActor, List<FinalDPS>> _dpsTarget = new Dictionary<AbstractMasterActor, List<FinalDPS>>();
        private List<FinalDPS> _dpsAll;
        private Dictionary<AbstractMasterActor, List<FinalStats>> _statsTarget = new Dictionary<AbstractMasterActor, List<FinalStats>>();
        private List<FinalStatsAll> _statsAll;
        private Dictionary<AbstractMasterActor, List<FinalDefenses>> _defensesTarget = new Dictionary<AbstractMasterActor, List<FinalDefenses>>();
        private List<FinalDefensesAll> _defensesAll;
        private Dictionary<AbstractMasterActor, List<FinalSupport>> _supportTarget = new Dictionary<AbstractMasterActor, List<FinalSupport>>();
        private List<FinalSupportAll> _support;
        //status
        private List<(long start, long end)> _deads;
        private List<(long start, long end)> _downs;
        private List<(long start, long end)> _dCs;

        protected AbstractSingleActor(AgentItem agent) : base(agent)
        {
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
        protected abstract void SetDamageLogs(ParsedLog log);
        public override List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractDamageEvent>();
                SetDamageLogs(log);
                DamageLogsByDst = DamageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageLogsByDst.TryGetValue(target.AgentItem, out var list))
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
                SetDamageTakenLogs(log);
                DamageTakenLogsBySrc = DamageTakenlogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenLogsBySrc.TryGetValue(target.AgentItem, out var list))
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
        protected void AddDamageLogs(List<AbstractDamageEvent> damageEvents)
        {
            DamageLogs.AddRange(damageEvents.Where(x => x.IFF != ParseEnum.IFF.Friend));
        }
        protected void SetDamageTakenLogs(ParsedLog log)
        {
            DamageTakenlogs.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
        }
        public List<AbstractDamageEvent> GetJustPlayerDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (!_selfDamageLogsPerTarget.TryGetValue(target ?? GeneralHelper.NullActor, out List<AbstractDamageEvent> dls))
            {
                dls = GetDamageLogs(target, log, start, end).Where(x => x.From == AgentItem).ToList();
                _selfDamageLogsPerTarget[target ?? GeneralHelper.NullActor] = dls;
            }
            return dls;
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

        protected BuffMap GetBuffMap(ParsedLog log)
        {
            //
            BuffMap buffMap = new BuffMap();
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
                foreach (var pair in buffMap)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(GeneralHelper.UnknownAgent, AgentItem, dsp.Time, int.MaxValue, log.SkillData.Get(pair.Key), 1, int.MaxValue));
                }
            }
            buffMap.Sort();
            foreach (var pair in buffMap)
            {
                TrackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
            }
            return buffMap;
        }

        // Buffs
        protected void InitBuffStatusData(ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BuffDistribution());
                _buffPresence.Add(new Dictionary<long, long>());
            }
        }

        protected void SetBuffStatusCleanseWasteData(ParsedLog log, BuffSimulator simulator, long boonid, bool updateCondiPresence)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            List<AbstractBuffSimulationItem> extraSimulations = new List<AbstractBuffSimulationItem>(simulator.OverstackSimulationResult);
            extraSimulations.AddRange(simulator.WasteSimulationResult);
            foreach (AbstractBuffSimulationItem simul in extraSimulations)
            {
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
                }
            }
        }
        protected void SetBuffStatusGenerationData(ParsedLog log, BuffSimulationItem simul, long boonid)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                Add(_buffPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
            }
        }
        public BuffDistribution GetBuffDistribution(ParsedLog log, int phaseIndex)
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
            BuffMap toUse = GetBuffMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            BuffsGraphModel boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfBoonsID]);
            BuffsGraphModel condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfConditionsID]);
            HashSet<long> boonIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Boon].Select(x => x.ID));
            HashSet<long> condiIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Condition].Select(x => x.ID));
            InitBuffStatusData(log);
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
                    List<BuffsGraphModel.SegmentWithSources> graphSegments = new List<BuffsGraphModel.SegmentWithSources>();
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        SetBuffStatusGenerationData(log, simul, boonid);
                        BuffsGraphModel.SegmentWithSources segment = simul.ToSegment();
                        if (graphSegments.Count == 0)
                        {
                            graphSegments.Add(new BuffsGraphModel.SegmentWithSources(0, segment.Start, 0, GeneralHelper.UnknownAgent));
                        }
                        else if (graphSegments.Last().End != segment.Start)
                        {
                            graphSegments.Add(new BuffsGraphModel.SegmentWithSources(graphSegments.Last().End, segment.Start, 0, GeneralHelper.UnknownAgent));
                        }
                        graphSegments.Add(segment);
                    }
                    SetBuffStatusCleanseWasteData(log, simulator, boonid, updateCondiPresence);
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new BuffsGraphModel.SegmentWithSources(graphSegments.Last().End, dur, 0, GeneralHelper.UnknownAgent));
                    }
                    else
                    {
                        graphSegments.Add(new BuffsGraphModel.SegmentWithSources(0, dur, 0, GeneralHelper.UnknownAgent));
                    }
                    BuffPoints[boonid] = new BuffsGraphModel(buff, graphSegments);
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
        protected List<FinalDPS> GetFinalDPS(ParsedLog log, AbstractMasterActor target)
        {
            List<FinalDPS> res = new List<FinalDPS>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                double phaseDuration = (phase.DurationInMS) / 1000.0;
                int damage;
                double dps = 0.0;
                FinalDPS final = new FinalDPS();
                List<AbstractDamageEvent> damageLogs = GetDamageLogs(target, log, phase.Start, phase.End);
                List<AbstractDamageEvent> damageLogsActor = GetJustPlayerDamageLogs(target, log, phase.Start, phase.End);
                //DPS
                damage = damageLogs.Sum(x => x.Damage);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.Dps = (int)Math.Round(dps);
                final.Damage = damage;
                //Condi DPS
                damage = damageLogs.Sum(x => x.IsCondi(log) ? x.Damage : 0);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.CondiDps = (int)Math.Round(dps);
                final.CondiDamage = damage;
                //Power DPS
                damage = final.Damage - final.CondiDamage;
                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.PowerDps = (int)Math.Round(dps);
                final.PowerDamage = damage;
                // Actor DPS
                damage = damageLogsActor.Sum(x => x.Damage);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.ActorDps = (int)Math.Round(dps);
                final.ActorDamage = damage;
                //Actor Condi DPS
                damage = damageLogsActor.Sum(x => x.IsCondi(log) ? x.Damage : 0);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.ActorCondiDps = (int)Math.Round(dps);
                final.ActorCondiDamage = damage;
                //Actor Power DPS
                damage = final.ActorDamage - final.ActorCondiDamage;
                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.ActorPowerDps = (int)Math.Round(dps);
                final.ActorPowerDamage = damage;
                res.Add(final);
            }
            return res;
        }

        public FinalDPS GetDPS(ParsedLog log, int phaseIndex)
        {
            if (_dpsAll == null)
            {
                _dpsAll = GetFinalDPS(log, null);
            }
            return _dpsAll[phaseIndex];
        }

        public List<FinalDPS> GetDPS(ParsedLog log)
        {
            if (_dpsAll == null)
            {
                _dpsAll = GetFinalDPS(log, null);
            }
            return _dpsAll;
        }

        public FinalDPS GetDPS(ParsedLog log, int phaseIndex, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_dpsTarget.ContainsKey(target))
            {
                _dpsTarget[target] = GetFinalDPS(log, target);
            }
            return _dpsTarget[target][phaseIndex];
        }

        public List<FinalDPS> GetDPS(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_dpsTarget.ContainsKey(target))
            {
                _dpsTarget[target] = GetFinalDPS(log, target);
            }
            return _dpsTarget[target];
        }
        // Stats
        public List<FinalStatsAll> GetStatsAll(ParsedLog log)
        {
            if (_statsAll == null)
            {
                _statsAll = GetStats(log);
            }
            return _statsAll;
        }

        public List<FinalStats> GetStatsTarget(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (_statsTarget == null)
            {
                _statsTarget[target] = GetStats(log, target);
            }
            return _statsTarget[target];
        }

        private void FillFinalStats(List<AbstractDamageEvent> dls, FinalStats final)
        {
            HashSet<long> nonCritable = new HashSet<long>
                    {
                        9292,
                        5492,
                        13014,
                        30770,
                        52370
                    };
            // (x - 1) / x
            foreach (AbstractDamageEvent dl in dls)
            {
                if (!(dl is NonDirectDamageEvent))
                {
                    if (dl.HasCrit)
                    {
                        final.CriticalCount++;
                        final.CriticalDmg += dl.Damage;
                    }

                    if (dl.IsFlanking)
                    {
                        final.FlankingCount++;
                    }

                    if (dl.HasGlanced)
                    {
                        final.GlanceCount++;
                    }

                    if (dl.IsBlind)
                    {
                        final.Missed++;
                    }
                    if (dl.HasInterrupted)
                    {
                        final.Interrupts++;
                    }

                    if (dl.IsAbsorbed)
                    {
                        final.Invulned++;
                    }
                    final.DirectDamageCount++;
                    if (!nonCritable.Contains(dl.SkillId))
                    {
                        final.CritableDirectDamageCount++;
                    }
                }
            }
        }

        private List<FinalStatsAll> GetStats(ParsedLog log)
        {
            List<FinalStatsAll> res = new List<FinalStatsAll>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];

                FinalStatsAll final = new FinalStatsAll();
                res.Add(final);
                FillFinalStats(GetJustPlayerDamageLogs(null, log, phase.Start, phase.End), final);
                // If conjured sword, stop
                if (IsFakeActor)
                {
                    continue;
                }
                foreach (AbstractCastEvent cl in GetCastLogs(log, phase.Start, phase.End))
                {
                    if (cl.Interrupted)
                    {
                        final.Wasted++;
                        final.TimeWasted += cl.ActualDuration;
                    }
                    if (cl.ReducedAnimation)
                    {
                        if (cl.ActualDuration < cl.ExpectedDuration)
                        {
                            final.Saved++;
                            final.TimeSaved += cl.ExpectedDuration - cl.ActualDuration;
                        }
                    }
                    if (cl.SkillId == SkillItem.WeaponSwapId)
                    {
                        final.SwapCount++;
                    }
                }
                final.TimeSaved = Math.Round(final.TimeSaved / 1000.0, GeneralHelper.TimeDigit);
                final.TimeWasted = Math.Round(final.TimeWasted / 1000.0, GeneralHelper.TimeDigit);

                double avgBoons = 0;
                foreach (long duration in GetBuffPresence(log, i).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BuffNature.Boon).Select(x => x.Value))
                {
                    avgBoons += duration;
                }
                final.AvgBoons = Math.Round(avgBoons / phase.DurationInMS, GeneralHelper.BoonDigit);
                long activeDuration = phase.GetActorActiveDuration(this, log);
                final.AvgActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, GeneralHelper.BoonDigit) : 0.0;

                double avgCondis = 0;
                foreach (long duration in GetBuffPresence(log, i).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BuffNature.Condition).Select(x => x.Value))
                {
                    avgCondis += duration;
                }
                final.AvgConditions = Math.Round(avgCondis / phase.DurationInMS, GeneralHelper.BoonDigit);
                final.AvgActiveConditions = activeDuration > 0 ? Math.Round(avgCondis / activeDuration, GeneralHelper.BoonDigit) : 0.0;
            }
            return res;
        }
        private List<FinalStats> GetStats(ParsedLog log, AbstractMasterActor target)
        {
            List<FinalStats> res = new List<FinalStats>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];

                FinalStats final = new FinalStatsAll();
                res.Add(final);
                FillFinalStats(GetJustPlayerDamageLogs(target, log, phase.Start, phase.End), final);
            }
            return res;
        }
        // Defenses
        public List<FinalDefensesAll> GetDefenses(ParsedLog log)
        {
            if (_defensesAll == null)
            {
                _defensesAll = GetFinalDefenses(log);
            }
            return _defensesAll;
        }

        public List<FinalDefenses> GetDefenses(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (_defensesTarget.ContainsKey(target))
            {
                _defensesTarget[target] = GetFinalDefenses(log, target);
            }
            return _defensesTarget[target];
        }

        private void FillFinalDefenses(FinalDefenses finalDefenses, ParsedLog log, long start, long end, AbstractMasterActor target)
        {

            List<AbstractDamageEvent> damageLogs = GetDamageTakenLogs(target, log, start, end);

            finalDefenses.DamageTaken = damageLogs.Sum(x => (long)x.Damage);
            finalDefenses.BlockedCount = damageLogs.Count(x => x.IsBlocked);
            finalDefenses.InvulnedCount = 0;
            finalDefenses.DamageInvulned = 0;
            finalDefenses.EvadedCount = damageLogs.Count(x => x.IsEvaded);
            finalDefenses.DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
            finalDefenses.InterruptedCount = damageLogs.Count(x => x.HasInterrupted);
            foreach (AbstractDamageEvent dl in damageLogs.Where(x => x.IsAbsorbed))
            {
                finalDefenses.InvulnedCount++;
                finalDefenses.DamageInvulned += dl.Damage;
            }
        }

        private List<FinalDefensesAll> GetFinalDefenses(ParsedLog log)
        {
            List<(long start, long end)> dead = new List<(long start, long end)>();
            List<(long start, long end)> down = new List<(long start, long end)>();
            List<(long start, long end)> dc = new List<(long start, long end)>();
            AgentItem.GetAgentStatus(dead, down, dc, log);
            List<FinalDefensesAll> res = new List<FinalDefensesAll>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                FinalDefensesAll final = new FinalDefensesAll();
                res.Add(final);
                long start = phase.Start;
                long end = phase.End;
                FillFinalDefenses(final, log, start, end, null);
                //	Commons	
                final.DodgeCount = GetCastLogs(log, start, end).Count(x => x.SkillId == SkillItem.DodgeId);
                final.DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);

                final.DownDuration = (int)down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DeadDuration = (int)dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DcDuration = (int)dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            }
            return res;
        }
        private List<FinalDefenses> GetFinalDefenses(ParsedLog log, AbstractMasterActor target)
        {
            List<FinalDefenses> res = new List<FinalDefenses>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                FinalDefensesAll final = new FinalDefensesAll();
                res.Add(final);
                long start = phase.Start;
                long end = phase.End;
                FillFinalDefenses(final, log, start, end, target);
            }
            return res;
        }
        // Support
        public List<FinalSupportAll> GetSupport(ParsedLog log)
        {
            if (_support == null)
            {
                _support = GetFinalSupport(log);
            }
            return _support;
        }
        public List<FinalSupport> GetSupport(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!_supportTarget.ContainsKey(target))
            {
                _supportTarget[target] = GetFinalSupport(log, target);
            }
            return _supportTarget[target];
        }
        private long[] GetCleanses(ParsedLog log, PhaseData phase, AbstractMasterActor target)
        {
            if (target == null)
            {
                return GetCleanses(log, phase);
            }
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Condition].Select(x => x.ID))
            {
                List<BuffRemoveAllEvent> bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && x.To == target.AgentItem).Select(x => x as BuffRemoveAllEvent).ToList();
                cleanse[0] += bevts.Count;
                cleanse[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return cleanse;
        }
        private long[] GetCleanses(ParsedLog log, PhaseData phase)
        {
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Condition].Select(x => x.ID))
            {
                List<BuffRemoveAllEvent> bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && x.To != GeneralHelper.UnknownAgent).Select(x => x as BuffRemoveAllEvent).ToList();
                cleanse[0] += bevts.Count;
                cleanse[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return cleanse;
        }

        private long[] GetBoonStrips(ParsedLog log, PhaseData phase)
        {
            long[] strips = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Boon].Select(x => x.ID))
            {
                List<BuffRemoveAllEvent> bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && x.To != GeneralHelper.UnknownAgent).Select(x => x as BuffRemoveAllEvent).ToList();
                strips[0] += bevts.Count;
                strips[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return strips;
        }
        private long[] GetBoonStrips(ParsedLog log, PhaseData phase, AbstractMasterActor target)
        {
            if (target == null)
            {
                return GetBoonStrips(log, phase);
            }
            long[] strips = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Boon].Select(x => x.ID))
            {
                List<BuffRemoveAllEvent> bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && x.To == target.AgentItem).Select(x => x as BuffRemoveAllEvent).ToList();
                strips[0] += bevts.Count;
                strips[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return strips;
        }

        private long[] GetReses(ParsedLog log, long start, long end)
        {
            List<AbstractCastEvent> cls = GetCastLogs(log, start, end);
            long[] reses = { 0, 0 };
            foreach (AbstractCastEvent cl in cls)
            {
                if (cl.SkillId == SkillItem.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.ActualDuration;
                }
            }
            return reses;
        }

        private void FillFinalSupport(FinalSupport finalSupport, ParsedLog log, PhaseData phase, AbstractMasterActor target)
        {
            long[] cleanseArray = GetCleanses(log, phase, target);
            long[] boonStrips = GetBoonStrips(log, phase, target);
            finalSupport.CondiCleanse = cleanseArray[0];
            finalSupport.CondiCleanseTime = cleanseArray[1] / 1000.0;
            finalSupport.BoonStrips = boonStrips[0];
            finalSupport.BoonStripsTime = boonStrips[1] / 1000.0;
        }

        private List<FinalSupportAll> GetFinalSupport(ParsedLog log)
        {
            List<FinalSupportAll> res = new List<FinalSupportAll>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                FinalSupportAll final = new FinalSupportAll();
                res.Add(final);
                PhaseData phase = phases[phaseIndex];

                long[] resArray = GetReses(log, phase.Start, phase.End);
                final.Resurrects = resArray[0];
                final.ResurrectTime = resArray[1] / 1000.0;
                FillFinalSupport(final, log, phase, null);
            }
            return res;
        }
        private List<FinalSupport> GetFinalSupport(ParsedLog log, AbstractMasterActor target)
        {
            List<FinalSupport> res = new List<FinalSupport>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                FinalSupport final = new FinalSupport();
                res.Add(final);
                PhaseData phase = phases[phaseIndex];
                FillFinalSupport(final, log, phase, target);
            }
            return res;
        }
    }
}
