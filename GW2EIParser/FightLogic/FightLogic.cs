﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public abstract class FightLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, WvW, Unknown };

        private CombatReplayMap _map;
        protected List<Mechanic> MechanicList { get; }
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public string Extension { get; protected set; }
        public string Icon { get; protected set; }
        private readonly int _basicMechanicsCount;
        public bool HasNoFightSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
        public List<NPC> NPCs { get; } = new List<NPC>();
        protected ushort TriggerID { get; }

        protected FightLogic(ushort triggerID)
        {
            TriggerID = triggerID;
            MechanicList = new List<Mechanic>() {
                new PlayerStatusMechanic(SkillItem.DeathId, "Dead", new MechanicPlotlySetting("x","rgb(0,0,0)"), "Dead",0),
                new PlayerStatusMechanic(SkillItem.DownId, "Downed", new MechanicPlotlySetting("cross","rgb(255,0,0)"), "Downed",0),
                new PlayerStatusMechanic(SkillItem.ResurrectId, "Resurrect", new MechanicPlotlySetting("cross-open","rgb(0,255,255)"), "Resurrect",0),
                new PlayerStatusMechanic(SkillItem.AliveId, "Got up", new MechanicPlotlySetting("cross","rgb(0,255,0)"), "Got up",0),
                new PlayerStatusMechanic(SkillItem.DCId, "Disconnected", new MechanicPlotlySetting("x","rgb(120,120,120)"), "Disconnect",0),
                new PlayerStatusMechanic(SkillItem.RespawnId, "Respawn", new MechanicPlotlySetting("cross","rgb(120,120,255)"), "Respawn",0)
            };
            _basicMechanicsCount = MechanicList.Count;
        }

        public MechanicData GetMechanicData()
        {
            return new MechanicData(MechanicList);
        }

        protected virtual CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("", (800, 800), (0, 0, 0, 0), (0, 0, 0, 0), (0, 0, 0, 0));
        }

        public CombatReplayMap GetCombatMap(ParsedLog log)
        {
            if (_map == null)
            {
                _map = GetCombatMapInternal();
                _map.ComputeBoundingBox(log);
            }
            return _map;
        }

        protected virtual List<ushort> GetFightNPCsIDs()
        {
            return new List<ushort>
            {
                TriggerID
            };
        }

        protected virtual HashSet<ushort> GetFriendlyNPCsIDs()
        {
            return new HashSet<ushort>();
        }

        public virtual string GetFightName()
        {
            NPC target = NPCs.Find(x => x.ID == TriggerID);
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }

        private static void RegroupNPCsByID(ushort id, AgentData agentData, List<CombatItem> combatItems)
        {
            List<AgentItem> agents = agentData.GetAgentsByID(id);
            if (agents.Count > 1)
            {
                AgentItem firstItem = agents.First();
                var agentValues = new HashSet<ulong>(agents.Select(x => x.Agent));
                var newTargetAgent = new AgentItem(firstItem)
                {
                    FirstAwareLogTime = agents.Min(x => x.FirstAwareLogTime),
                    LastAwareLogTime = agents.Max(x => x.LastAwareLogTime)
                };
                // get unique id for the fusion
                ushort instID = 0;
                var rnd = new Random();
                while (agentData.InstIDValues.Contains(instID) || instID == 0)
                {
                    instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
                }
                newTargetAgent.InstID = instID;
                agentData.OverrideID(id, newTargetAgent);
                foreach (CombatItem c in combatItems)
                {
                    if (agentValues.Contains(c.SrcAgent))
                    {
                        c.OverrideSrcValues(newTargetAgent.Agent, newTargetAgent.InstID);
                    }
                    if (agentValues.Contains(c.DstAgent))
                    {
                        c.OverrideDstValues(newTargetAgent.Agent, newTargetAgent.InstID);
                    }
                }
            }
        }

        protected abstract HashSet<ushort> GetUniqueTargetIDs();

        protected void ComputeFightNPCs(AgentData agentData, List<CombatItem> combatItems)
        {
            foreach (ushort id in GetUniqueTargetIDs())
            {
                RegroupNPCsByID(id, agentData, combatItems);
            }
            List<ushort> ids = GetFightNPCsIDs();
            HashSet<ushort> friendlies = GetFriendlyNPCsIDs();
            var aList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => ids.Contains(x.ID)).ToList();
            foreach (AgentItem a in aList)
            {
                bool friendly = friendlies.Contains(a.ID);
                if (a.MasterAgent != null)
                {
                    AgentItem master = a.MasterAgent;
                    while (master.MasterAgent != null && !ids.Contains(master.ID))
                    {
                        master = master.MasterAgent;
                    }
                    if (!ids.Contains(a.MasterAgent.ID))
                    {
                        var masterNPC = new NPC(a.MasterAgent, friendly);
                        NPCs.Add(masterNPC);
                    }
                    continue;
                }
                var target = new NPC(a, friendly);
                NPCs.Add(target);
            }
        }

        protected static List<PhaseData> GetPhasesByInvul(ParsedLog log, long skillID, NPC mainTarget, bool addSkipPhases, bool beginWithStart)
        {
            long fightDuration = log.FightData.FightDuration;
            var phases = new List<PhaseData>();
            long last = 0;
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, skillID, mainTarget, beginWithStart);
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent c = invuls[i];
                if (c is BuffApplyEvent)
                {
                    long end = c.Time;
                    phases.Add(new PhaseData(last, end));
                    /*if (i == invuls.Count - 1)
                    {
                        mainTarget.AddCustomCastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None, log);
                    }*/
                    last = end;
                }
                else
                {
                    long end = c.Time;
                    if (addSkipPhases)
                    {
                        phases.Add(new PhaseData(last, end));
                    }
                    //mainTarget.AddCustomCastLog(last, -5, (int)(end - last), ParseEnum.Activation.None, (int)(end - last), ParseEnum.Activation.None, log);
                    last = end;
                }
            }
            if (fightDuration - last > 5000)
            {
                phases.Add(new PhaseData(last, fightDuration));
            }
            return phases;
        }

        protected static List<PhaseData> GetInitialPhase(ParsedLog log)
        {
            var phases = new List<PhaseData>();
            long fightDuration = log.FightData.FightDuration;
            phases.Add(new PhaseData(0, fightDuration));
            phases[0].Name = "Full Fight";
            return phases;
        }

        public virtual List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = NPCs.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            return phases;
        }

        protected void AddTargetsToPhase(PhaseData phase, List<ushort> ids, ParsedLog log)
        {
            foreach (NPC target in NPCs)
            {
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(log.FightData.ToFightSpace(target.FirstAwareLogTime), 0)))
                {
                    phase.Targets.Add(target);
                }
            }
            phase.OverrideTimes(log);
        }

        public virtual List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<AgentItem, List<AbstractBuffEvent>> buffsBySrc, Dictionary<long, List<AbstractBuffEvent>> buffsById, long offset, SkillData skillData)
        {
            return new List<AbstractBuffEvent>();
        }

        protected static void NegateDamageAgainstBarrier(List<AgentItem> agentItems, Dictionary<AgentItem, List<AbstractDamageEvent>> damageByDst)
        {
            var dmgEvts = new List<AbstractDamageEvent>();
            foreach (AgentItem agentItem in agentItems)
            {
                if (damageByDst.TryGetValue(agentItem, out var list))
                {
                    dmgEvts.AddRange(list);
                }
            }
            foreach (AbstractDamageEvent de in dmgEvts)
            {
                if (de.ShieldDamage > 0)
                {
                    de.NegateDamage();
                }
            }
        }

        public virtual List<AbstractDamageEvent> SpecialDamageEventProcess(Dictionary<AgentItem, List<AbstractDamageEvent>> damageBySrc, Dictionary<AgentItem, List<AbstractDamageEvent>> damageByDst, Dictionary<long, List<AbstractDamageEvent>> damageById, long offset, SkillData skillData)
        {
            return new List<AbstractDamageEvent>();
        }

        public virtual void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
        }

        public virtual void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
        }

        protected int HPBasedCM(CombatData combatData, AgentData agentData, ushort id, double hpValue)
        {
            NPC target = NPCs.Find(x => x.ID == id);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > hpValue) ? 1 : 0;
        }

        public virtual int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return -1;
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, bool all, ushort idFirst, params ushort[] ids)
        {
            var idsToUse = new List<ushort>
            {
                idFirst
            };
            idsToUse.AddRange(ids);
            SetSuccessByDeath(combatData, fightData, playerAgents, all, idsToUse);
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, bool all, List<ushort> idsToUse)
        {
            int success = 0;
            long maxTime = long.MinValue;
            foreach (ushort id in idsToUse)
            {
                NPC target = NPCs.Find(x => x.ID == id);
                if (target == null)
                {
                    return;
                }
                DeadEvent killed = combatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                if (killed != null)
                {
                    long time = killed.Time;
                    success++;
                    AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.MasterFrom)));
                    if (lastDamageTaken != null)
                    {
                        time = Math.Min(lastDamageTaken.Time, time);
                    }
                    maxTime = Math.Max(time, maxTime);
                }
            }
            if ((all && success == idsToUse.Count) || (!all && success > 0))
            {
                fightData.SetSuccess(true, fightData.ToLogSpace(maxTime));
            }
        }

        protected void SetSuccessByCombatExit(HashSet<ushort> targetIds, CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            var targets = NPCs.Where(x => targetIds.Contains(x.ID)).ToList();
            SetSuccessByCombatExit(targets, combatData, fightData, playerAgents);
        }

        protected static void SetSuccessByCombatExit(List<NPC> targets, CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            if (targets.Count == 0)
            {
                return;
            }
            var playerExits = new List<ExitCombatEvent>();
            var targetExits = new List<ExitCombatEvent>();
            var lastTargetDamages = new List<AbstractDamageEvent>();
            foreach (AgentItem a in playerAgents)
            {
                playerExits.AddRange(combatData.GetExitCombatEvents(a));
            }
            foreach (NPC t in targets)
            {
                EnterCombatEvent enterCombat = combatData.GetEnterCombatEvents(t.AgentItem).LastOrDefault();
                if (enterCombat != null)
                {
                    targetExits.AddRange(combatData.GetExitCombatEvents(t.AgentItem).Where(x => x.Time > enterCombat.Time));
                }
                else
                {
                    targetExits.AddRange(combatData.GetExitCombatEvents(t.AgentItem));
                }
                AbstractDamageEvent lastDamage = combatData.GetDamageTakenData(t.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.MasterFrom)));
                if (lastDamage != null)
                {
                    lastTargetDamages.Add(lastDamage);
                }
            }
            ExitCombatEvent lastPlayerExit = playerExits.Count > 0 ? playerExits.MaxBy(x => x.Time) : null;
            ExitCombatEvent lastTargetExit = targetExits.Count > 0 ? targetExits.MaxBy(x => x.Time) : null;
            AbstractDamageEvent lastDamageTaken = lastTargetDamages.Count > 0 ? lastTargetDamages.MaxBy(x => x.Time) : null;
            if (lastTargetExit != null && lastDamageTaken != null)
            {
                if (lastPlayerExit != null)
                {
                    fightData.SetSuccess(lastPlayerExit.Time > lastTargetExit.Time + 1000, fightData.ToLogSpace(lastDamageTaken.Time));
                }
                else if (fightData.FightEndLogTime > targets.Max(x => x.LastAwareLogTime) + 2000)
                {
                    fightData.SetSuccess(true, fightData.ToLogSpace(lastDamageTaken.Time));
                }
            }
        }

        public virtual void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, true, TriggerID);
        }

        public virtual void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            ComputeFightNPCs(agentData, combatData);
        }

        //
        protected static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AbstractSingleActor target, bool beginWithStart)
        {
            return GetFilteredList(combatData, buffID, target.AgentItem, beginWithStart);
        }
        protected static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AgentItem target, bool beginWithStart)
        {
            bool needStart = beginWithStart;
            var main = combatData.GetBuffData(buffID).Where(x => x.To == target && (x is BuffApplyEvent || x is BuffRemoveAllEvent)).ToList();
            var filtered = new List<AbstractBuffEvent>();
            for (int i = 0; i < main.Count; i++)
            {
                AbstractBuffEvent c = main[i];
                if (needStart && c is BuffApplyEvent)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c is BuffRemoveAllEvent)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1] is BuffApplyEvent))
                    {
                        needStart = true;
                        filtered.Add(c);
                    }
                }
            }
            return filtered;
        }

    }
}
