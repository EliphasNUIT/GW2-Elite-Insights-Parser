﻿using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffOverstackItem : BuffWasteItem
    {
        public BuffOverstackItem(AgentItem src, long overstack, long time) : base(src, overstack, time, 0)
        {
        }
    }
}
