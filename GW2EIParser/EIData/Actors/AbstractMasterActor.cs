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
        private readonly Dictionary<AbstractActor, List<AbstractDamageEvent>> _selfDamageLogsPerTarget = new Dictionary<AbstractActor, List<AbstractDamageEvent>>();
        // Minions
        private Dictionary<string, Minions> _minions;
        // Statistics
        protected Dictionary<AbstractMasterActor, List<FinalDPS>> DpsTarget = new Dictionary<AbstractMasterActor, List<FinalDPS>>();
        protected List<FinalDPS> DpsAll;
        protected Dictionary<AbstractMasterActor, List<FinalStats>> StatsTarget = new Dictionary<AbstractMasterActor, List<FinalStats>>();
        protected List<FinalStatsAll> StatsAll;
        protected Dictionary<AbstractMasterActor, List<FinalDefenses>> DefensesTarget = new Dictionary<AbstractMasterActor, List<FinalDefenses>>();
        protected List<FinalDefensesAll> DefensesAll;
        protected Dictionary<AbstractMasterActor, List<FinalSupport>> SupportTarget = new Dictionary<AbstractMasterActor, List<FinalSupport>>();
        protected List<FinalSupportAll> Support;
        // Replay
        protected CombatReplay CombatReplay;

        protected AbstractMasterActor(AgentItem agent) : base(agent)
        {

        }
        // Damage logs
        public List<AbstractDamageEvent> GetJustPlayerDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (!_selfDamageLogsPerTarget.TryGetValue(target ?? GeneralHelper.NullActor, out List<AbstractDamageEvent> dls))
            {
                dls = GetDamageLogs(target, log, start, end).Where(x => x.From == AgentItem).ToList();
                _selfDamageLogsPerTarget[target ?? GeneralHelper.NullActor] = dls;
            }
            return dls;
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
        // Minions
        public Dictionary<string, Minions> GetMinions(ParsedLog log)
        {
            if (_minions == null)
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
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightDuration).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightDuration).Count > 0)
                    {
                        _minions[pair.Key] = pair.Value;
                    }
                }
            }
            return _minions;
        }
        // Buffs
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

        public FinalDPS GetDPS(ParsedLog log, int phaseIndex)
        {
            if (DpsAll == null)
            {
                DpsAll = GetFinalDPS(log, null);
            }
            return DpsAll[phaseIndex];
        }

        public List<FinalDPS> GetDPS(ParsedLog log)
        {
            if (DpsAll == null)
            {
                DpsAll = GetFinalDPS(log, null);
            }
            return DpsAll;
        }

        public FinalDPS GetDPS(ParsedLog log, int phaseIndex, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!DpsTarget.ContainsKey(target))
            {
                DpsTarget[target] = GetFinalDPS(log, target);
            }
            return DpsTarget[target][phaseIndex];
        }

        public List<FinalDPS> GetDPS(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!DpsTarget.ContainsKey(target))
            {
                DpsTarget[target] = GetFinalDPS(log, target);
            }
            return DpsTarget[target];
        }
        // Stats
        public List<FinalStatsAll> GetStatsAll(ParsedLog log)
        {
            if (StatsAll == null)
            {
                StatsAll = GetStats(log);
            }
            return StatsAll;
        }

        public List<FinalStats> GetStatsTarget(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (StatsTarget == null)
            {
                StatsTarget[target] = GetStats(log, target);
            }
            return StatsTarget[target];
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
                foreach (long duration in GetBuffPresence(log, i).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BoonNature.Boon).Select(x => x.Value))
                {
                    avgBoons += duration;
                }
                final.AvgBoons = Math.Round(avgBoons / phase.DurationInMS, GeneralHelper.BoonDigit);
                long activeDuration = phase.GetActorActiveDuration(this, log);
                final.AvgActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, GeneralHelper.BoonDigit) : 0.0;

                double avgCondis = 0;
                foreach (long duration in GetBuffPresence(log, i).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BoonNature.Condition).Select(x => x.Value))
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
            if (DefensesAll == null)
            {
                DefensesAll = GetFinalDefenses(log);
            }
            return DefensesAll;
        }

        public List<FinalDefenses> GetDefenses(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (DefensesTarget.ContainsKey(target))
            {
                DefensesTarget[target] = GetFinalDefenses(log, target);
            }
            return DefensesTarget[target];
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
            if (Support == null)
            {
                Support = GetFinalSupport(log);
            }
            return Support;
        }
        public List<FinalSupport> GetSupport(ParsedLog log, AbstractMasterActor target)
        {
            if (target == null)
            {
                throw new InvalidOperationException("Target can't be null");
            }
            if (!SupportTarget.ContainsKey(target))
            {
                SupportTarget[target] = GetFinalSupport(log, target);
            }
            return SupportTarget[target];
        }
        private long[] GetCleanses(ParsedLog log, PhaseData phase, AbstractMasterActor target)
        {
            if (target == null)
            {
                return GetCleanses(log, phase);
            }
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BoonsByNature[Buff.BoonNature.Condition].Select(x => x.ID))
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
            foreach (long id in log.Buffs.BoonsByNature[Buff.BoonNature.Condition].Select(x => x.ID))
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
            foreach (long id in log.Buffs.BoonsByNature[Buff.BoonNature.Boon].Select(x => x.ID))
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
            foreach (long id in log.Buffs.BoonsByNature[Buff.BoonNature.Boon].Select(x => x.ID))
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