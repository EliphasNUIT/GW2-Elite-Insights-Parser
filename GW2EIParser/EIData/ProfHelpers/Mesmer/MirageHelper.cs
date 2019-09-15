using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class MirageHelper : MesmerHelper
    {
        public static List<AnimatedCastEvent> TranslateMirageCloak(List<AbstractBuffEvent> buffs, SkillData skillData)
        {
            var res = new List<AnimatedCastEvent>();
            long cloakStart = 0;
            foreach (AbstractBuffEvent ba in buffs)
            {
                if (!(ba is BuffApplyEvent))
                {
                    continue;
                }
                if (ba.Time - cloakStart > 10)
                {
                    var dodgeLog = new AnimatedCastEvent(ba.Time, skillData.Get(SkillItem.MirageCloakDodgeId), 50, ba.To.MasterAgent ?? ba.To);
                    res.Add(dodgeLog);
                    cloakStart = ba.Time;
                }
            }
            return res;
        }
    }
}
