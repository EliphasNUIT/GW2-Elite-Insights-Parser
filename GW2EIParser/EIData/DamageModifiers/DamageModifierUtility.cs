using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
//using static GW2EIParser.Builders.JsonModels.JsonStatistics;
using static GW2EIParser.EIData.Player;

namespace GW2EIParser.EIData
{
    public static class DamageModifierUtility
    {
        public enum DamageType { All, Power, Condition };
        public enum DamageSource { All, NoPets, PetsOnly };
        public enum ModifierSource
        {
            CommonBuff,
            ItemBuff,
            Necromancer, Reaper, Scourge,
            Elementalist, Tempest, Weaver,
            Mesmer, Chronomancer, Mirage,
            Warrior, Berserker, Spellbreaker,
            Revenant, Herald, Renegade,
            Guardian, Dragonhunter, Firebrand,
            Thief, Daredevil, Deadeye,
            Ranger, Druid, Soulbeast,
            Engineer, Scrapper, Holosmith
        };


        public class DamageModifierMetaData
        {
            public DamageType SrcType { get; }
            public DamageSource DmgSrc { get; }
            public ModifierSource Src { get; }

            public DamageModifierMetaData(DamageType srcType, DamageSource dmgSrc, ModifierSource src)
            {
                SrcType = srcType;
                DmgSrc = dmgSrc;
                Src = src;
            }
        }

        // All meta Data

        public static readonly DamageModifierMetaData AllAllCommon = new DamageModifierMetaData(DamageType.All, DamageSource.All,ModifierSource.CommonBuff);

    }
}
