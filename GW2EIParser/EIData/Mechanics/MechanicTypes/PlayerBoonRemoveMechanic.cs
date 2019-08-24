using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class PlayerBoonRemoveMechanic : BoonRemoveMechanic
    {

        public PlayerBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BoonRemoveChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public PlayerBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BoonRemoveChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(SkillId))
                {
                    if (c is BuffRemoveManualEvent rme && p.AgentItem == rme.To && Keep(rme, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(rme.Time, this, p));
                    }
                }
            }
        }
    }
}
