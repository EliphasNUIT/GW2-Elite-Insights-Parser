using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.Models.Statistics;

namespace GW2EIParser.EIData
{
    public abstract class AbstractMasterActor : AbstractActor
    {
        public bool IsFakeActor { get; protected set; }
        // Boons
        private readonly List<BuffDistribution> _boonDistribution = new List<BuffDistribution>();
        private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        // damage list
        private List<AbstractDamageEvent> _selfDamageLogs;
        private readonly Dictionary<AbstractActor, List<AbstractDamageEvent>> _selfDamageLogsPerTarget = new Dictionary<AbstractActor, List<AbstractDamageEvent>>();
        // Minions
        private Dictionary<string, Minions> _minions;
        // Statistics
        protected List<FinalDPS> DpsAll;
        protected Dictionary<AbstractMasterActor, List<FinalDPS>> DpsTarget;
        protected Dictionary<AbstractMasterActor, List<FinalStats>> StatsTarget;
        protected List<FinalStatsAll> StatsAll;
        protected Dictionary<AbstractMasterActor, List<FinalDefenses>> DefensesTarget;
        protected List<FinalDefenses> DefensesAll;
        protected Dictionary<AbstractMasterActor, List<FinalSupport>> SupportTarget;
        protected List<FinalSupport> Support;
        // Replay
        protected CombatReplay CombatReplay;

        protected AbstractMasterActor(AgentItem agent) : base(agent)
        {

        }
        // Minions
        public Dictionary<string, Minions> GetMinions(ParsedLog log)
        {
            if (_minions == null)
            {
                SetMinions(log);
            }
            return _minions;
        }
        // Buffs
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

        public FinalDPS GetDPSAll(ParsedLog log, int phaseIndex)
        {
            if (DpsAll == null)
            {
                DpsAll = GetFinalDPS(log, null);
            }
            return DpsAll[phaseIndex];
        }

        public List<FinalDPS> GetDPSAll(ParsedLog log)
        {
            if (DpsAll == null)
            {
                DpsAll = GetFinalDPS(log, null);
            }
            return DpsAll;
        }

        public FinalDPS GetDPSTarget(ParsedLog log, int phaseIndex, AbstractMasterActor target)
        {
            if (!DpsTarget.ContainsKey(target))
            {
                DpsTarget[target] = GetFinalDPS(log, target);
            }
            if (target == null)
            {
                return GetDPSAll(log, phaseIndex);
            }
            return DpsTarget[target][phaseIndex];
        }

        public List<FinalDPS> GetDPSTarget(ParsedLog log, AbstractMasterActor target)
        {
            if (!DpsTarget.ContainsKey(target))
            {
                DpsTarget[target] = GetFinalDPS(log, target);
            }
            if (target == null)
            {
                return GetDPSAll(log);
            }
            return DpsTarget[target];
        }
        // Stats
        public List<FinalStatsAll> GetStatsAll(ParsedLog log)
        {
            if (StatsAll == null)
            {
                SetStats(log);
            }
            return StatsAll;
        }

        public List<FinalStats> GetStatsTarget(ParsedLog log, AbstractMasterActor target)
        {
            if (StatsTarget == null)
            {
                SetStats(log);
            }
            if (target == null)
            {
                return new List<FinalStats>(GetStatsAll(log));
            }
            return StatsTarget[target];
        }

        private void FillFinalStats(List<AbstractDamageEvent> dls, FinalStats final, Dictionary<AbstractMasterActor, FinalStats> targetsFinal)
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
                    foreach (var pair in targetsFinal)
                    {
                        AbstractMasterActor target = pair.Key;
                        if (dl.To == target.AgentItem)
                        {
                            FinalStats targetFinal = pair.Value;
                            if (dl.HasCrit)
                            {
                                targetFinal.CriticalCount++;
                                targetFinal.CriticalDmg += dl.Damage;
                            }

                            if (dl.IsFlanking)
                            {
                                targetFinal.FlankingCount++;
                            }

                            if (dl.HasGlanced)
                            {
                                targetFinal.GlanceCount++;
                            }

                            if (dl.IsBlind)
                            {
                                targetFinal.Missed++;
                            }
                            if (dl.HasInterrupted)
                            {
                                targetFinal.Interrupts++;
                            }

                            if (dl.IsAbsorbed)
                            {
                                targetFinal.Invulned++;
                            }
                            targetFinal.DirectDamageCount++;
                            if (!nonCritable.Contains(dl.SkillId))
                            {
                                targetFinal.CritableDirectDamageCount++;
                            }
                        }
                    }
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

        private void SetStats(ParsedLog log)
        {
            int phaseIndex = -1;
            StatsAll = new List<FinalStatsAll>();
            StatsTarget = new Dictionary<AbstractMasterActor, List<FinalStats>>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                phaseIndex++;
                Dictionary<AbstractMasterActor, FinalStats> targetDict = new Dictionary<AbstractMasterActor, FinalStats>();
                foreach (Target target in log.FightData.Logic.Targets)
                {
                    if (!StatsTarget.ContainsKey(target))
                    {
                        StatsTarget[target] = new List<FinalStats>();
                    }
                    StatsTarget[target].Add(new FinalStats());
                    targetDict[target] = StatsTarget[target].Last();
                }
                FinalStatsAll final = new FinalStatsAll();
                FillFinalStats(GetJustPlayerDamageLogs(null, log, phase.Start, phase.End), final, targetDict);
                StatsAll.Add(final);
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
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BoonNature.Boon).Select(x => x.Value))
                {
                    avgBoons += duration;
                }
                final.AvgBoons = Math.Round(avgBoons / phase.DurationInMS, GeneralHelper.BoonDigit);
                long activeDuration = phase.GetActorActiveDuration(this, log);
                final.AvgActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, GeneralHelper.BoonDigit) : 0.0;

                double avgCondis = 0;
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BoonNature.Condition).Select(x => x.Value))
                {
                    avgCondis += duration;
                }
                final.AvgConditions = Math.Round(avgCondis / phase.DurationInMS, GeneralHelper.BoonDigit);
                final.AvgActiveConditions = activeDuration > 0 ? Math.Round(avgCondis / activeDuration, GeneralHelper.BoonDigit) : 0.0;

                if (log.CombatData.HasMovementData && this is Player)
                {
                    if (CombatReplay == null)
                    {
                        InitCombatReplay(log);
                    }
                    List<Point3D> positions = CombatReplay.PolledPositions.Where(x => x.Time >= phase.Start && x.Time <= phase.End).ToList();
                    List<Point3D> stackCenterPositions = log.Statistics.GetStackCenterPositions(log);
                    int offset = CombatReplay.PolledPositions.Count(x => x.Time < phase.Start);
                    if (positions.Count > 1)
                    {
                        List<float> distances = new List<float>();
                        for (int time = 0; time < positions.Count; time++)
                        {

                            float deltaX = positions[time].X - stackCenterPositions[time + offset].X;
                            float deltaY = positions[time].Y - stackCenterPositions[time + offset].Y;
                            //float deltaZ = positions[time].Z - StackCenterPositions[time].Z;


                            distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                        }
                        final.StackDist = distances.Sum() / distances.Count;
                    }
                    else
                    {
                        final.StackDist = -1;
                    }
                }
            }
        }
        // Defenses
        public List<FinalDefenses> GetDefensesAll(ParsedLog log)
        {
            if (DefensesAll == null)
            {
                SetDefenses(log);
            }
            return DefensesAll;
        }

        public List<FinalDefenses> GetDefenses(ParsedLog log, AbstractMasterActor target)
        {
            if (DefensesTarget == null)
            {
                SetDefenses(log);
            }
            if (target == null)
            {
                return GetDefensesAll(log);
            }
            return DefensesAll;
        }

        private void SetDefenses(ParsedLog log)
        {
            List<(long start, long end)> dead = new List<(long start, long end)>();
            List<(long start, long end)> down = new List<(long start, long end)>();
            List<(long start, long end)> dc = new List<(long start, long end)>();
            AgentItem.GetAgentStatus(dead, down, dc, log);
            DefensesAll = new List<FinalDefenses>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                FinalDefenses final = new FinalDefenses();
                DefensesAll.Add(final);
                long start = phase.Start;
                long end = phase.End;
                List<AbstractDamageEvent> damageLogs = GetDamageTakenLogs(null, log, start, end);
                //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());

                final.DamageTaken = damageLogs.Sum(x => (long)x.Damage);
                //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                final.BlockedCount = damageLogs.Count(x => x.IsBlocked);
                final.InvulnedCount = 0;
                final.DamageInvulned = 0;
                final.EvadedCount = damageLogs.Count(x => x.IsEvaded);
                final.DodgeCount = GetCastLogs(log, start, end).Count(x => x.SkillId == SkillItem.DodgeId);
                final.DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
                final.InterruptedCount = damageLogs.Count(x => x.HasInterrupted);
                foreach (AbstractDamageEvent dl in damageLogs.Where(x => x.IsAbsorbed))
                {
                    final.InvulnedCount++;
                    final.DamageInvulned += dl.Damage;
                }

                //		
                final.DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);

                final.DownDuration = (int)down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DeadDuration = (int)dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DcDuration = (int)dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            }
        }
        // Support
        public List<FinalSupport> GetSupport(ParsedLog log)
        {
            if (Support == null)
            {
                SetSupport(log);
            }
            return Support;
        }

        private long[] GetCleansesNotSelf(ParsedLog log, PhaseData phase)
        {
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BoonsByNature[Buff.BoonNature.Condition].Select(x => x.ID))
            {
                List<BuffRemoveAllEvent> bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && x.To != AgentItem).Select(x => x as BuffRemoveAllEvent).ToList();
                cleanse[0] += bevts.Count;
                cleanse[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return cleanse;
        }
        private long[] GetCleansesSelf(ParsedLog log, PhaseData phase)
        {
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BoonsByNature[Buff.BoonNature.Condition].Select(x => x.ID))
            {
                List<BuffRemoveAllEvent> bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && x.To == AgentItem).Select(x => x as BuffRemoveAllEvent).ToList();
                cleanse[0] += bevts.Count;
                cleanse[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return cleanse;
        }

        private long[] GetBoonStrips(ParsedLog log, PhaseData phase)
        {
            long[] strips = { 0, 0 };
            foreach (long id in log.Buffs.BoonsByNature[Buff.BoonNature.Boon].Select(x => x.ID))
            {
                List<BuffRemoveAllEvent> bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && !log.PlayerAgents.Contains(x.To) && !log.PlayerAgents.Contains(x.To.MasterAgent)).Select(x => x as BuffRemoveAllEvent).ToList();
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

        private void SetSupport(ParsedLog log)
        {
            Support = new List<FinalSupport>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                FinalSupport final = new FinalSupport();
                Support.Add(final);
                PhaseData phase = phases[phaseIndex];

                long[] resArray = GetReses(log, phase.Start, phase.End);
                long[] cleanseArray = GetCleansesNotSelf(log, phase);
                long[] cleanseSelfArray = GetCleansesSelf(log, phase);
                long[] boonStrips = GetBoonStrips(log, phase);
                final.Resurrects = resArray[0];
                final.ResurrectTime = resArray[1] / 1000.0;
                final.CondiCleanse = cleanseArray[0];
                final.CondiCleanseTime = cleanseArray[1] / 1000.0;
                final.CondiCleanseSelf = cleanseSelfArray[0];
                final.CondiCleanseTimeSelf = cleanseSelfArray[1] / 1000.0;
                final.BoonStrips = boonStrips[0];
                final.BoonStripsTime = boonStrips[1] / 1000.0;
            }
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

        public List<Point3D> GetCombatReplayActivePositions(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.GetActivePositions();
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

        public List<GenericActor> GetCombatReplayActors(ParsedLog log)
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
        // Damage logs
        public List<AbstractDamageEvent> GetJustPlayerDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (!_selfDamageLogsPerTarget.TryGetValue(target??GeneralHelper.NullActor, out List<AbstractDamageEvent> dls))
            {
                dls = GetDamageLogs(target, log, start, end).Where(x => x.From == AgentItem).ToList();
                _selfDamageLogsPerTarget[target ?? GeneralHelper.NullActor] = dls;
            }
            return dls;
        }

        // private setters

        protected override void InitBuffStatusData(ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BuffDistribution());
                _buffPresence.Add(new Dictionary<long, long>());
            }
        }

        protected override void SetBuffStatusCleanseWasteData(ParsedLog log, BuffSimulator simulator, long boonid, bool updateCondiPresence)
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

        protected override void SetBuffStatusGenerationData(ParsedLog log, BuffSimulationItem simul, long boonid)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                Add(_buffPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
            }
        }

        private void SetMinions(ParsedLog log)
        {
            _minions = new Dictionary<string, Minions>();
            List<AgentItem> combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.MasterAgent == AgentItem).ToList();
            Dictionary<string, Minions> auxMinions = new Dictionary<string, Minions>();
            foreach (AgentItem agent in combatMinion)
            {
                string id = agent.Name;
                if (!auxMinions.ContainsKey(id))
                {
                    auxMinions[id] = new Minions(id.GetHashCode());
                }
                auxMinions[id].Add(new Minion(agent));
            }
            foreach (KeyValuePair<string, Minions> pair in auxMinions)
            {
                if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightDuration).Count > 0 || pair.Value.GetCastLogs(log,0, log.FightData.FightDuration).Count > 0)
                {
                    _minions[pair.Key] = pair.Value;
                }
            }
        }
        protected override void SetDamageLogs(ParsedLog log)
        {
            AddDamageLogs(log.CombatData.GetDamageData(AgentItem));
            Dictionary<string, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                DamageLogs.AddRange(mins.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
            }
            DamageLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        public int GetCombatReplayID(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return (InstID + "_" + CombatReplay.TimeOffsets.start + "_" + CombatReplay.TimeOffsets.end).GetHashCode();
        }

        // abstracts
        protected abstract void InitAdditionalCombatReplayData(ParsedLog log);


        public abstract class AbstractMasterActorSerializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public double[] Positions { get; set; }
        }

        public abstract AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);
    }
}