﻿namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class AbstractCombatEvent
    {
        public long Time { get; protected set; }
#if DEBUG
        protected CombatItem OriginalCombatEvent;
#endif

        protected AbstractCombatEvent(long logTime, long offset)
        {
            Time = logTime - offset;
        }
    }
}
