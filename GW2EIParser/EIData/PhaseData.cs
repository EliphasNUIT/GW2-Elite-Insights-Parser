﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class PhaseData
    {
        public long Start { get; private set; }
        public long End { get; private set; }
        public long DurationInS { get; private set; }
        public long DurationInMS { get; private set; }
        public long DurationInM { get; private set; }
        public string Name { get; set; }
        public List<AbstractSingleActor> Targets { get; } = new List<AbstractSingleActor>();

        public PhaseData(long start, long end)
        {
            Start = start;
            End = end;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        public bool InInterval(long time)
        {
            return Start <= time && time <= End;
        }

        public void OverrideStart(long start)
        {
            Start = start;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        public void OverrideEnd(long end)
        {
            End = end;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        /// <summary>
        /// Override times in a manner that the phase englobes the targets present in the phase (if possible)
        /// </summary>
        /// <param name="log"></param>
        public void OverrideTimes(ParsedLog log)
        {
            if (Targets.Count > 0)
            {
                Start = Math.Max(Start, log.FightData.ToFightSpace(Targets.Min(x => x.FirstAwareLogTime)));
                long end = long.MinValue;
                foreach (NPC target in Targets)
                {
                    long deadTime = log.FightData.ToFightSpace(target.LastAwareLogTime);
                    DeadEvent died = log.CombatData.GetDeadEvents(target.AgentItem).FirstOrDefault();
                    if (died != null)
                    {
                        deadTime = died.Time;
                    }
                    end = Math.Max(end, deadTime);
                }
                End = Math.Min(Math.Min(End, end), log.FightData.FightDuration);
            }
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        public long GetActorActiveDuration(AbstractSingleActor actor, ParsedLog log)
        {
            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            actor.AgentItem.GetAgentStatus(dead, down, dc, log);
            return DurationInMS -
                dead.Sum(x =>
                {
                    if (x.start <= End && x.end >= Start)
                    {
                        long s = Math.Max(x.start, Start);
                        long e = Math.Min(x.end, End);
                        return e - s;
                    }
                    return 0;
                }) -
                dc.Sum(x =>
                {
                    if (x.start <= End && x.end >= Start)
                    {
                        long s = Math.Max(x.start, Start);
                        long e = Math.Min(x.end, End);
                        return e - s;
                    }
                    return 0;
                });
        }
    }
}

