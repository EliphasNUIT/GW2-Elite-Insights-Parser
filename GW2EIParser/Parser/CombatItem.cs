﻿namespace GW2EIParser.Parser
{
    public class CombatItem
    {
        public long LogTime { get; private set; }
        public ulong SrcAgent { get; private set; }
        public ulong DstAgent { get; private set; }
        public int Value { get; private set; }
        public int BuffDmg { get; }
        public uint OverstackValue { get; }
        public long SkillID { get; }
        public ushort SrcInstid { get; private set; }
        public ushort DstInstid { get; private set; }
        public ushort SrcMasterInstid { get; private set; }
        public ushort DstMasterInstid { get; private set; }
        public ParseEnum.EvtcIFF IFF { get; }
        public byte IsBuff { get; }
        public byte Result { get; }
        public ParseEnum.EvtcActivation IsActivation { get; }
        public ParseEnum.EvtcBuffRemove IsBuffRemove { get; }
        public byte IsNinety { get; }
        public byte IsFifty { get; }
        public byte IsMoving { get; }
        public ParseEnum.EvtcStateChange IsStateChange { get; }
        public byte IsFlanking { get; }
        public byte IsShields { get; }
        public byte IsOffcycle { get; }

        // Constructor
        public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
               long skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid,
               ushort dstMasterInstid, ParseEnum.EvtcIFF iff, byte isBuff,
               byte result, ParseEnum.EvtcActivation isActivation,
               ParseEnum.EvtcBuffRemove isBuffRemove, byte isNinety, byte isFifty, byte isMoving,
               ParseEnum.EvtcStateChange isStateChange, byte isFlanking, byte isShields, byte isOffcycle)
        {
            LogTime = time;
            SrcAgent = srcAgent;
            DstAgent = dstAgent;
            Value = value;
            BuffDmg = buffDmg;
            OverstackValue = overstackValue;
            SkillID = skillId;
            SrcInstid = srcInstid;
            DstInstid = dstInstid;
            SrcMasterInstid = srcMasterInstid;
            DstMasterInstid = dstMasterInstid;
            IFF = iff;
            IsBuff = isBuff;
            Result = result;
            IsActivation = isActivation;
            IsBuffRemove = isBuffRemove;
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoving = isMoving;
            IsStateChange = isStateChange;
            IsFlanking = isFlanking;
            IsShields = isShields;
            IsOffcycle = isOffcycle;
        }

        public CombatItem(CombatItem c)
        {
            LogTime = c.LogTime;
            SrcAgent = c.SrcAgent;
            DstAgent = c.DstAgent;
            Value = c.Value;
            BuffDmg = c.BuffDmg;
            OverstackValue = c.OverstackValue;
            SkillID = c.SkillID;
            SrcInstid = c.SrcInstid;
            DstInstid = c.DstInstid;
            SrcMasterInstid = c.SrcMasterInstid;
            DstMasterInstid = c.DstMasterInstid;
            IFF = c.IFF;
            IsBuff = c.IsBuff;
            Result = c.Result;
            IsActivation = c.IsActivation;
            IsBuffRemove = c.IsBuffRemove;
            IsNinety = c.IsNinety;
            IsFifty = c.IsFifty;
            IsMoving = c.IsMoving;
            IsStateChange = c.IsStateChange;
            IsFlanking = c.IsFlanking;
            IsShields = c.IsShields;
            IsOffcycle = c.IsOffcycle;
        }


        public void OverrideTime(long time)
        {
            LogTime = time;
        }

        public void OverrideSrcValues(ulong agent, ushort instid)
        {
            SrcInstid = instid;
            SrcAgent = agent;
        }

        public void OverrideSrcValues(ulong agent, ushort instid, ushort masterInstid)
        {
            SrcInstid = instid;
            SrcAgent = agent;
            SrcMasterInstid = masterInstid;
        }

        public void OverrideDstValues(ulong agent, ushort instid)
        {
            DstInstid = instid;
            DstAgent = agent;
        }

        public void OverrideDstValues(ulong agent, ushort instid, ushort masterInstid)
        {
            DstInstid = instid;
            DstAgent = agent;
            DstMasterInstid = masterInstid;
        }

        public void OverrideValue(int value)
        {
            Value = value;
        }
    }
}