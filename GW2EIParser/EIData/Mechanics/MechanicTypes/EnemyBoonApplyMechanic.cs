﻿using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class EnemyBoonApplyMechanic : BoonApplyMechanic
    {
        public EnemyBoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BoonApplyChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyBoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BoonApplyChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public EnemyBoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyBoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs)
        {
            foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(SkillId))
            {
                AbstractSingleActor amp = null;
                if (c is BuffApplyEvent ba && Keep(ba, log))
                {
                    amp = log.FindActor(ba.To, false);
                }
                if (amp != null)
                {
                    mechanicLogs[this].Add(new MechanicEvent(c.Time, this, amp));
                }
            }
        }
    }
}