﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;
using static GW2EIParser.Models.BuffStatistics;

namespace GW2EIParser.EIData
{
    public class NPC : AbstractSingleActor
    {
        private List<Dictionary<long, FinalTargetBuffs>> _buffs;
        private int _health = -1;
        // Constructors
        public NPC(AgentItem agent, bool friendly) : base(agent)
        {
            Friendly = friendly;
        }

        public void OverrideName(string name)
        {
            Character = name;
        }
        // Health
        public int GetHealth(CombatData combatData)
        {
            if (_health == -1)
            {
                List<MaxHealthUpdateEvent> maxHpUpdates = combatData.GetMaxHealthUpdateEvents(AgentItem);
                _health = maxHpUpdates.Count > 0 ? maxHpUpdates.Max(x => x.MaxHealth) : 1;
            }
            return _health;
        }
        public void SetManualHealth(int health)
        {
            _health = health;
        }


        // Buffs
        public Dictionary<long, FinalTargetBuffs> GetBuffs(ParsedLog log, int phaseIndex)
        {
            if (_buffs == null)
            {
                SetBuffs(log);
            }
            return _buffs[phaseIndex];
        }

        public List<Dictionary<long, FinalTargetBuffs>> GetBuffs(ParsedLog log)
        {
            if (_buffs == null)
            {
                SetBuffs(log);
            }
            return _buffs;
        }

        private void SetBuffs(ParsedLog log)
        {
            _buffs = new List<Dictionary<long, FinalTargetBuffs>>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                BuffDistributionDictionary boonDistribution = GetBuffDistribution(log, phaseIndex);
                var rates = new Dictionary<long, FinalTargetBuffs>();
                _buffs.Add(rates);
                Dictionary<long, long> buffPresence = GetBuffPresence(log, phaseIndex);

                PhaseData phase = phases[phaseIndex];
                long fightDuration = phase.DurationInMS;

                foreach (Buff boon in TrackedBuffs)
                {
                    if (boonDistribution.ContainsKey(boon.ID))
                    {
                        var buff = new FinalTargetBuffs(log.PlayerList);
                        rates[boon.ID] = buff;
                        if (boon.Type == Buff.BuffType.Duration)
                        {
                            buff.Uptime = Math.Round(100.0 * boonDistribution.GetUptime(boon.ID) / fightDuration, GeneralHelper.BoonDigit);
                            foreach (Player p in log.PlayerList)
                            {
                                long gen = boonDistribution.GetGeneration(boon.ID, p.AgentItem);
                                buff.Generated[p] = Math.Round(100.0 * gen / fightDuration, GeneralHelper.BoonDigit);
                                buff.Overstacked[p] = Math.Round(100.0 * (boonDistribution.GetOverstack(boon.ID, p.AgentItem) + gen) / fightDuration, GeneralHelper.BoonDigit);
                                buff.Wasted[p] = Math.Round(100.0 * boonDistribution.GetWaste(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                                buff.UnknownExtension[p] = Math.Round(100.0 * boonDistribution.GetUnknownExtension(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                                buff.Extension[p] = Math.Round(100.0 * boonDistribution.GetExtension(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                                buff.Extended[p] = Math.Round(100.0 * boonDistribution.GetExtended(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            }
                        }
                        else if (boon.Type == Buff.BuffType.Intensity)
                        {
                            buff.Uptime = Math.Round((double)boonDistribution.GetUptime(boon.ID) / fightDuration, GeneralHelper.BoonDigit);
                            foreach (Player p in log.PlayerList)
                            {
                                long gen = boonDistribution.GetGeneration(boon.ID, p.AgentItem);
                                buff.Generated[p] = Math.Round((double)gen / fightDuration, GeneralHelper.BoonDigit);
                                buff.Overstacked[p] = Math.Round((double)(boonDistribution.GetOverstack(boon.ID, p.AgentItem) + gen) / fightDuration, GeneralHelper.BoonDigit);
                                buff.Wasted[p] = Math.Round((double)boonDistribution.GetWaste(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                                buff.UnknownExtension[p] = Math.Round((double)boonDistribution.GetUnknownExtension(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                                buff.Extension[p] = Math.Round((double)boonDistribution.GetExtension(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                                buff.Extended[p] = Math.Round((double)boonDistribution.GetExtended(boon.ID, p.AgentItem) / fightDuration, GeneralHelper.BoonDigit);
                            }
                            if (buffPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                            {
                                buff.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, GeneralHelper.BoonDigit);
                            }
                        }
                    }
                }
            }
        }
        // Combat Replay
        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            log.FightData.Logic.ComputeNPCCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Actors.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        public override JsonAbstractSingleActorCombatReplay GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            var aux = new JsonTargetCombatReplay
            {
                Img = CombatReplay.Icon,
                ID = AgentItem.UniqueID,
                Start = CombatReplay.TimeOffsets.start,
                End = CombatReplay.TimeOffsets.end,
                Positions = new List<double>()
            };
            foreach (Point3D pos in CombatReplay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions.Add(x);
                aux.Positions.Add(y);
            }
            return aux;
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CombatData.HasMovementData)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay
            {
                Icon = GeneralHelper.GetNPCIcon(ID)
            };
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightDuration, log.FightData.GetMainTargets(log).Contains(this));
            TrimCombatReplay(log);
        }
    }
}
