using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class GuildEvent : AbstractStatusEvent
    {
        public List<byte> Guid { get; }

        public GuildEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            byte[] first8 = BitConverter.GetBytes(evtcItem.DstAgent);
            byte[] mid4 = BitConverter.GetBytes(evtcItem.Value);
            byte[] last4 = BitConverter.GetBytes(evtcItem.BuffDmg);
            byte[] guid = new byte[first8.Length + mid4.Length + last4.Length];
            first8.CopyTo(guid, 0);
            mid4.CopyTo(guid, first8.Length);
            last4.CopyTo(guid, first8.Length + mid4.Length);
            Guid = guid.ToList();
        }

    }
}
