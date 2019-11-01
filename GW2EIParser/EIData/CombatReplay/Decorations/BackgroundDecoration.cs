using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public abstract class BackgroundDecoration : GenericDecoration
    {
        public BackgroundDecoration((int start, int end) lifespan) : base(lifespan, null)
        {
        }

        public abstract override JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);
    }
}
