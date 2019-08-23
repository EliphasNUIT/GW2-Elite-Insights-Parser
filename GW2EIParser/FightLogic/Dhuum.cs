﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.EvtcTrashIDS;

namespace GW2EIParser.Logic
{
    public class Dhuum : RaidLogic
    {
        private bool _isBugged;
        private short _reapersSeen;
        private int _greenStart;

        public Dhuum(ushort triggerID) : base(triggerID)
        {
            _isBugged = false;
            _reapersSeen = -7;
            _greenStart = 0;
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(48172, "Hateful Ephemera", new MechanicPlotlySetting("square","rgb(255,140,0)"), "Golem","Hateful Ephemera (Golem AoE dmg)", "Golem Dmg",0),
            new HitOnPlayerMechanic(48121, "Arcing Affliction", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Bomb dmg","Arcing Affliction (Bomb) hit", "Bomb dmg",0),
            new PlayerBoonApplyMechanic(47646, "Arcing Affliction", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Bomb","Arcing Affliction (Bomb) application", "Bomb",0),
            new PlayerBoonRemoveMechanic(47646, "Arcing Affliction", new MechanicPlotlySetting("diamond","rgb(255,0,0)"), "Bomb Trig","Arcing Affliction (Bomb) manualy triggered", "Bomb Triggered",0, (br, log) => br.RemovedDuration > 50 && !log.CombatData.GetDamageDataById(48210).Exists(x => Math.Abs(x.Time - br.Time) < 15 && x.To == br.To) && !br.To.HasBuff(log, 48281, br.Time)),
            //new Mechanic(47476, "Residual Affliction", ParseEnum.BossIDS.Dhuum, new MechanicPlotlySetting("star-diamond","rgb(255,200,0)"), "Bomb",0), //not needed, imho, applied at the same time as Arcing Affliction
            new PlayerOnPlayerMechanic(47335, "Soul Shackle", new MechanicPlotlySetting("diamond","rgb(0,255,255)"), "Shackles","Soul Shackle (Tether) application", "Shackles",0),//  //also used for removal.
            new HitOnPlayerMechanic(47164, "Soul Shackle", new MechanicPlotlySetting("diamond-open","rgb(0,255,255)"), "Shackles dmg","Soul Shackle (Tether) dmg ticks", "Shackles Dmg",0,   (de,log) => de.Damage > 0),
            new HitOnPlayerMechanic(47561, "Slash", new MechanicPlotlySetting("triangle","rgb(0,128,0)"), "Cone","Boon ripping Cone Attack", "Cone",0),
            new HitOnPlayerMechanic(48752, "Cull", new MechanicPlotlySetting("asterisk-open","rgb(0,255,255)"), "Crack","Cull (Fearing Fissures)", "Cracks",0),
            new HitOnPlayerMechanic(48760, "Putrid Bomb", new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Mark","Necro Marks during Scythe attack", "Necro Marks",0),
            new HitOnPlayerMechanic(48398, "Cataclysmic Cycle", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Suck dmg","Damage when sucked to close to middle", "Suck dmg",0),
            new HitOnPlayerMechanic(48176, "Death Mark", new MechanicPlotlySetting("hexagon","rgb(255,140,0)"), "Dip","Lesser Death Mark hit (Dip into ground)", "Dip AoE",0),
            new HitOnPlayerMechanic(48210, "Greater Death Mark", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "KB dmg","Knockback damage during Greater Deathmark (mid port)", "Knockback dmg",0),
          //  new Mechanic(48281, "Mortal Coil", ParseEnum.BossIDS.Dhuum, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Green Orbs",
            new PlayerBoonApplyMechanic(46950, "Fractured Spirit", new MechanicPlotlySetting("square","rgb(0,255,0)"), "Orb CD","Applied when taking green", "Green port",0),
            //new SkillOnPlayerMechanic(47076 , "Echo's Damage", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Echo","Damaged by Ender's Echo (pick up)", "Ender's Echo",5000),
            new PlayerBoonApplyMechanic(49125, "Echo's Pick up", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Echo PU","Picked up by Ender's Echo", "Ender's Pick up", 3000),
            new PlayerBoonRemoveMechanic(49125, "Freed from Echo", new MechanicPlotlySetting("square","rgb(0,0,255)"), "F Echo","Freed from Ender's Echo", "Freed from Echo", 0, (br,log) => log.CombatData.GetDeadEvents(br.To).Where(x => Math.Abs(x.Time - br.Time) <= 20).Count() == 0)
            });
            Extension = "dhuum";
            IconUrl = "https://wiki.guildwars2.com/images/e/e4/Mini_Dhuum.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/CLTwWBJ.png",
                            (3763, 3383),
                            (13524, -1334, 18039, 2735),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        private void ComputeFightPhases(List<PhaseData> phases, List<AbstractCastEvent> castLogs, long fightDuration, long start)
        {
            AbstractCastEvent shield = castLogs.Find(x => x.SkillId == 47396);
            if (shield != null)
            {
                long end = shield.Time;
                phases.Add(new PhaseData(start, end));
                AbstractCastEvent firstDamage = castLogs.FirstOrDefault(x => x.SkillId == 47304 && x.Time >= end);
                if (firstDamage != null)
                {
                    phases.Add(new PhaseData(firstDamage.Time, fightDuration));
                }
            }
            else
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
        }

        private List<PhaseData> GetInBetweenSoulSplits(ParsedLog log, Target dhuum, long mainStart, long mainEnd, bool hasRitual)
        {
            List<AbstractCastEvent> cls = dhuum.GetCastLogs(log, 0, log.FightData.FightDuration);
            List<AbstractCastEvent> cataCycle = cls.Where(x => x.SkillId == 48398).ToList();
            List<AbstractCastEvent> gDeathmark = cls.Where(x => x.SkillId == 48210).ToList();
            if (gDeathmark.Count < cataCycle.Count)
            {
                return new List<PhaseData>();
            }
            List<PhaseData> phases = new List<PhaseData>();
            long start = mainStart;
            long end = 0;
            int i = 1;
            foreach (AbstractCastEvent cl in cataCycle)
            {
                AbstractCastEvent clDeathmark = gDeathmark[i - 1];
                end = Math.Min(clDeathmark.Time, mainEnd);
                phases.Add(new PhaseData(start, end)
                {
                    Name = "Pre-Soulsplit " + i++
                });
                start = cl.Time + cl.ActualDuration;
            }
            phases.Add(new PhaseData(start, mainEnd)
            {
                Name = hasRitual ? "Pre-Ritual" : "Pre-Wipe"
            });
            foreach (PhaseData phase in phases)
            {
                phase.Targets.Add(dhuum);
            }
            phases.RemoveAll(x => x.DurationInMS <= 2200);
            return phases;
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.EvtcTargetIDS.Dhuum);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Sometimes the preevent is not in the evtc
            List<AbstractCastEvent> castLogs = mainTarget.GetCastLogs(log, 0, log.FightData.FightDuration);
            List<AbstractCastEvent> dhuumCast = mainTarget.GetCastLogs(log, 0, 20000);
            string[] namesDh;
            if (dhuumCast.Count > 0)
            {
                namesDh = new[] { "Main Fight", "Ritual" };
                ComputeFightPhases(phases, castLogs, fightDuration, 0);
                _isBugged = true;
            }
            else
            {
                AbstractBuffEvent invulDhuum = log.CombatData.GetBuffData(762).FirstOrDefault(x => x is BuffRemoveManualEvent && x.To == mainTarget.AgentItem && x.Time > 115000);
                if (invulDhuum != null)
                {
                    long end = invulDhuum.Time;
                    phases.Add(new PhaseData(0, end));
                    ComputeFightPhases(phases, castLogs, fightDuration, end + 1);
                }
                else
                {
                    phases.Add(new PhaseData(0, fightDuration));
                }
                namesDh = new[] { "Roleplay", "Main Fight", "Ritual" };
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = namesDh[i - 1];
                phases[i].Targets.Add(mainTarget);
            }
            bool hasRitual = phases.Last().Name == "Ritual";
            if (dhuumCast.Count > 0 && phases.Count > 1)
            {
                phases.AddRange(GetInBetweenSoulSplits(log, mainTarget, phases[1].Start, phases[1].End, hasRitual));
                phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            }
            else if (phases.Count > 2)
            {
                phases.AddRange(GetInBetweenSoulSplits(log, mainTarget, phases[2].Start, phases[2].End, hasRitual));
                phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            }
            return phases;
        }

        protected override List<ParseEnum.EvtcTrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.EvtcTrashIDS>
            {
                Echo,
                Enforcer,
                Messenger,
                Deathling,
                UnderworldReaper,
                DhuumDesmina
            };
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            // TODO: correct position
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.EvtcTargetIDS.Dhuum:
                    List<AbstractCastEvent> deathmark = cls.Where(x => x.SkillId == 48176).ToList();
                    AbstractCastEvent majorSplit = cls.Find(x => x.SkillId == 47396);
                    foreach (AbstractCastEvent c in deathmark)
                    {
                        int start = (int)c.Time;
                        int zoneActive = start + 1550;
                        int zoneDeadly = zoneActive + 6000; //point where the zone becomes impossible to walk through unscathed
                        int zoneEnd = zoneActive + 120000;
                        int radius = 450;
                        if (majorSplit != null)
                        {
                            zoneEnd = Math.Min(zoneEnd, (int)majorSplit.Time);
                            zoneDeadly = Math.Min(zoneDeadly, (int)majorSplit.Time);
                        }
                        int spellCenterDistance = 200; //hitbox radius
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 3000);
                        Point3D targetPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= start + 3000);
                        if (facing != null && targetPosition != null)
                        {
                            Point3D position = new Point3D(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z, targetPosition.Time);
                            replay.Actors.Add(new CircleDecoration(true, zoneActive, radius, (start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleDecoration(false, 0, radius, (start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleDecoration(true, 0, radius, (zoneActive, zoneDeadly), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleDecoration(true, 0, radius, (zoneDeadly, zoneEnd), "rgba(255, 100, 0, 0.5)", new PositionConnector(position)));

                        }
                    }
                    List<AbstractCastEvent> cataCycle = cls.Where(x => x.SkillId == 48398).ToList();
                    foreach (AbstractCastEvent c in cataCycle)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleDecoration(true, end, 300, (start, end), "rgba(255, 150, 0, 0.7)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleDecoration(true, 0, 300, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    }
                    List<AbstractCastEvent> slash = cls.Where(x => x.SkillId == 47561).ToList();
                    foreach (AbstractCastEvent c in slash)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Actors.Add(new PieDecoration(false, 0, 850, facing, 60, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    }

                    if (majorSplit != null)
                    {
                        int start = (int)majorSplit.Time;
                        int end = (int)log.FightData.FightDuration;
                        replay.Actors.Add(new CircleDecoration(true, 0, 320, (start, end), "rgba(0, 180, 255, 0.2)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }

        }

        public override void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (mob.ID)
            {
                case (ushort)DhuumDesmina:
                    break;
                case (ushort)Echo:
                    replay.Actors.Add(new CircleDecoration(true, 0, 120, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Enforcer:
                    break;
                case (ushort)Messenger:
                    replay.Actors.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Deathling:
                    break;
                case (ushort)UnderworldReaper:
                    // if not bugged and we assumed we are still on the reapers at the door, check if start is above 2 seconds (first reaper spawns around 10+ seconds). If yes, put _reapersSeen at 0 to start greens. 
                    if (!_isBugged && _reapersSeen < 0 && start > 2000)
                    {
                        //Reminder that agents appear in chronological order, after this one, reaper has spawned afer the first one
                        _reapersSeen = 0;
                    }
                    if (!_isBugged && _reapersSeen >= 0)
                    {
                        if (_greenStart == 0)
                        {
                            AbstractBuffEvent greenTaken = log.CombatData.GetBuffData(46950).Where(x => x is BuffApplyEvent).FirstOrDefault();
                            if (greenTaken != null)
                            {
                                _greenStart = (int)greenTaken.Time - 5000;
                            }
                            else
                            {
                                _greenStart = 30600;
                            }
                        }
                        int multiplier = 210000;
                        int gStart = _greenStart + _reapersSeen * 30000;
                        List<int> greens = new List<int>() {
                            gStart,
                            gStart + multiplier,
                            gStart + 2 * multiplier
                        };
                        foreach (int gstart in greens)
                        {
                            int gend = gstart + 5000;
                            replay.Actors.Add(new CircleDecoration(true, 0, 240, (gstart, gend), "rgba(0, 255, 0, 0.2)", new AgentConnector(mob)));
                            replay.Actors.Add(new CircleDecoration(true, gend, 240, (gstart, gend), "rgba(0, 255, 0, 0.2)", new AgentConnector(mob)));
                        }
                    }
                    List<AbstractBuffEvent> stealths = GetFilteredList(log.CombatData, 13017, mob, true);
                    int stealthStart = 0;
                    int stealthEnd = 0;
                    foreach (AbstractBuffEvent c in stealths)
                    {
                        if (c is BuffApplyEvent)
                        {
                            stealthStart = (int)c.Time;
                        }
                        else
                        {
                            stealthEnd = (int)c.Time;
                            replay.Actors.Add(new CircleDecoration(true, 0, 180, (stealthStart, stealthEnd), "rgba(80, 80, 80, 0.3)", new AgentConnector(mob)));
                        }
                    }
                    _reapersSeen++;
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");

            }
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // spirit transform
            List<AbstractBuffEvent> spiritTransform = log.CombatData.GetBuffData(46950).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.EvtcTargetIDS.Dhuum);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            foreach (AbstractBuffEvent c in spiritTransform)
            {
                int duration = 15000;
                HealthUpdateEvent hpUpdate = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem).FirstOrDefault(x => x.Time > c.Time);
                if (hpUpdate != null && hpUpdate.HPPercent < 10.50)
                {
                    duration = 30000;
                }
                AbstractBuffEvent removedBuff = log.CombatData.GetBuffData(48281).FirstOrDefault(x => x.To == p.AgentItem && x is BuffRemoveAllEvent && x.Time > c.Time && x.Time < c.Time + duration);
                int start = (int)c.Time;
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)removedBuff.Time;
                }
                replay.Actors.Add(new CircleDecoration(true, 0, 100, (start, end), "rgba(0, 50, 200, 0.3)", new AgentConnector(p)));
                replay.Actors.Add(new CircleDecoration(true, start + duration, 100, (start, end), "rgba(0, 50, 200, 0.5)", new AgentConnector(p)));
            }
            // bomb
            List<AbstractBuffEvent> bombDhuum = GetFilteredList(log.CombatData, 47646, p, true);
            int bombDhuumStart = 0;
            foreach (AbstractBuffEvent c in bombDhuum)
            {
                if (c is BuffApplyEvent)
                {
                    bombDhuumStart = (int)c.Time;
                }
                else
                {
                    int bombDhuumEnd = (int)c.Time;
                    replay.Actors.Add(new CircleDecoration(true, 0, 100, (bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.3)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleDecoration(true, bombDhuumStart + 13000, 100, (bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.5)", new AgentConnector(p)));
                }
            }
            // shackles connection
            List<AbstractBuffEvent> shackles = GetFilteredList(log.CombatData, 47335, p, true).Concat(GetFilteredList(log.CombatData, 48591, p, true)).ToList();
            int shacklesStart = 0;
            Player shacklesTarget = null;
            foreach (AbstractBuffEvent c in shackles)
            {
                if (c is BuffApplyEvent)
                {
                    shacklesStart = (int)c.Time;
                    shacklesTarget = log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By);
                }
                else
                {
                    int shacklesEnd = (int)c.Time;
                    if (shacklesTarget != null)
                    {
                        replay.Actors.Add(new LineDecoration(0, (shacklesStart, shacklesEnd), "rgba(0, 255, 255, 0.5)", new AgentConnector(p), new AgentConnector(shacklesTarget)));
                    }
                }
            }
            // shackles damage (identical to the connection for now, not yet properly distinguishable from the pure connection, further investigation needed due to inconsistent behavior (triggering too early, not triggering the damaging skill though)
            // shackles start with buff 47335 applied from one player to the other, this is switched over to buff 48591 after mostly 2 seconds, sometimes later. This is switched to 48042 usually 4 seconds after initial application and the damaging skill 47164 starts to deal damage from that point on.
            // Before that point, 47164 is only logged when evaded/blocked, but doesn't deal damage. Further investigation needed.
            List<AbstractBuffEvent> shacklesDmg = GetFilteredList(log.CombatData, 48042, p, true);
            int shacklesDmgStart = 0;
            Player shacklesDmgTarget = null;
            foreach (AbstractBuffEvent c in shacklesDmg)
            {
                if (c is BuffApplyEvent)
                {
                    shacklesDmgStart = (int)c.Time;
                    shacklesDmgTarget = log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By);
                }
                else
                {
                    int shacklesDmgEnd = (int)c.Time;
                    if (shacklesDmgTarget != null)
                    {
                        replay.Actors.Add(new LineDecoration(0, (shacklesDmgStart, shacklesDmgEnd), "rgba(0, 255, 255, 0.5)", new AgentConnector(p), new AgentConnector(shacklesDmgTarget)));
                    }
                }
            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.EvtcTargetIDS.Dhuum);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 35e6) ? 1 : 0;
        }
    }
}
