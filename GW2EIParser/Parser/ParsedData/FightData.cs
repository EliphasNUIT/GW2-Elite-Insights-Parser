using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Logic;

namespace GW2EIParser.Parser.ParsedData
{
    public class FightData
    {
        // Fields
        private List<PhaseData> _phases;
        public ushort ID { get; }
        private readonly bool _requirePhases;
        public FightLogic Logic { get; }
        public long FightStartLogTime { get; private set; }
        public long FightEndLogTime { get; private set; } = long.MaxValue;
        public long FightDuration => FightEndLogTime - FightStartLogTime;
        public string DurationString
        {
            get
            {
                var duration = TimeSpan.FromMilliseconds(FightDuration);
                string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s " + duration.Milliseconds + "ms";
                if (duration.Hours > 0)
                {
                    durationString = duration.ToString("hh") + "h " + durationString;
                }
                return durationString;
            }
        }
        public bool Success { get; private set; }
        public string Name => Logic.GetFightName() + (_isCM == 1 ? " CM" : "");
        private int _isCM = -1;
        public bool IsCM => _isCM == 1;
        // Constructors
        public FightData(ushort id, AgentData agentData, long start, long end)
        {
            FightStartLogTime = start;
            FightEndLogTime = end;
            ID = id;
            _requirePhases = Properties.Settings.Default.ParsePhases;
            switch (ParseEnum.GetNPCIDS(id))
            {
                case ParseEnum.NPCIDs.ValeGuardian:
                    Logic = new ValeGuardian(id);
                    break;
                case ParseEnum.NPCIDs.Gorseval:
                    Logic = new Gorseval(id);
                    break;
                case ParseEnum.NPCIDs.Sabetha:
                    Logic = new Sabetha(id);
                    break;
                case ParseEnum.NPCIDs.Slothasor:
                    Logic = new Slothasor(id);
                    break;
                case ParseEnum.NPCIDs.Zane:
                case ParseEnum.NPCIDs.Berg:
                case ParseEnum.NPCIDs.Narella:
                    Logic = new BanditTrio(id);
                    break;
                case ParseEnum.NPCIDs.Matthias:
                    Logic = new Matthias(id);
                    break;
                /*case ParseEnum.TargetIDS.Escort:
                    Logic = new Escort(id, agentData);
                    break;*/
                case ParseEnum.NPCIDs.KeepConstruct:
                    Logic = new KeepConstruct(id);
                    break;
                case ParseEnum.NPCIDs.Xera:
                    Logic = new Xera(id);
                    break;
                case ParseEnum.NPCIDs.Cairn:
                    Logic = new Cairn(id);
                    break;
                case ParseEnum.NPCIDs.MursaatOverseer:
                    Logic = new MursaatOverseer(id);
                    break;
                case ParseEnum.NPCIDs.Samarog:
                    Logic = new Samarog(id);
                    break;
                case ParseEnum.NPCIDs.Deimos:
                    Logic = new Deimos(id);
                    break;
                case ParseEnum.NPCIDs.SoullessHorror:
                    Logic = new SoullessHorror(id);
                    break;
                case ParseEnum.NPCIDs.Desmina:
                    Logic = new River(id);
                    break;
                case ParseEnum.NPCIDs.BrokenKing:
                    Logic = new BrokenKing(id);
                    break;
                case ParseEnum.NPCIDs.SoulEater:
                    Logic = new EaterOfSouls(id);
                    break;
                case ParseEnum.NPCIDs.EyeOfFate:
                case ParseEnum.NPCIDs.EyeOfJudgement:
                    Logic = new DarkMaze(id);
                    break;
                case ParseEnum.NPCIDs.Dhuum:
                    // some eyes log are registered as Dhuum
                    if (agentData.GetAgentsByID((ushort)ParseEnum.NPCIDs.EyeOfFate).Count > 0 ||
                        agentData.GetAgentsByID((ushort)ParseEnum.NPCIDs.EyeOfJudgement).Count > 0)
                    {
                        ID = (ushort)ParseEnum.NPCIDs.EyeOfFate;
                        Logic = new DarkMaze((ushort)ParseEnum.NPCIDs.EyeOfFate);
                        break;
                    }
                    Logic = new Dhuum(id);
                    break;
                case ParseEnum.NPCIDs.ConjuredAmalgamate:
                    Logic = new ConjuredAmalgamate(id);
                    break;
                case ParseEnum.NPCIDs.Kenut:
                case ParseEnum.NPCIDs.Nikare:
                    Logic = new TwinLargos(id);
                    break;
                case ParseEnum.NPCIDs.Qadim:
                    Logic = new Qadim(id);
                    break;
                case ParseEnum.NPCIDs.Freezie:
                    Logic = new Freezie(id);
                    break;
                case ParseEnum.NPCIDs.Adina:
                    Logic = new Adina(id);
                    break;
                case ParseEnum.NPCIDs.Sabir:
                    Logic = new Sabir(id);
                    break;
                case ParseEnum.NPCIDs.PeerlessQadim:
                    Logic = new PeerlessQadim(id);
                    break;
                case ParseEnum.NPCIDs.IcebroodConstruct:
                    Logic = new IcebroodConstruct(id);
                    break;
                case ParseEnum.NPCIDs.MAMA:
                    Logic = new MAMA(id);
                    break;
                case ParseEnum.NPCIDs.Siax:
                    Logic = new Siax(id);
                    break;
                case ParseEnum.NPCIDs.Ensolyss:
                    Logic = new Ensolyss(id);
                    break;
                case ParseEnum.NPCIDs.Skorvald:
                    Logic = new Skorvald(id);
                    break;
                case ParseEnum.NPCIDs.Artsariiv:
                    Logic = new Artsariiv(id);
                    break;
                case ParseEnum.NPCIDs.Arkk:
                    Logic = new Arkk(id);
                    break;
                case ParseEnum.NPCIDs.WorldVersusWorld:
                    Logic = new WvWFight(id);
                    break;
                case ParseEnum.NPCIDs.MassiveGolem:
                case ParseEnum.NPCIDs.AvgGolem:
                case ParseEnum.NPCIDs.LGolem:
                case ParseEnum.NPCIDs.MedGolem:
                case ParseEnum.NPCIDs.StdGolem:
                    Logic = new Golem(id);
                    break;
                default:
                    // Unknown
                    Logic = new UnknownFightLogic(id);
                    break;
            }
        }

        public List<PhaseData> GetPhases(ParsedLog log)
        {
            if (_phases == null)
            {
                long fightDuration = log.FightData.FightDuration;
                _phases = log.FightData.Logic.GetPhases(log, _requirePhases);
            }
            _phases.RemoveAll(x => x.DurationInMS <= 1000);
            return _phases;
        }

        public List<AbstractSingleActor> GetMainTargets(ParsedLog log)
        {
            if (_phases == null)
            {
                _phases = log.FightData.Logic.GetPhases(log, _requirePhases);
            }
            return _phases[0].Targets;
        }

        public long ToFightSpace(long time)
        {
            return time - FightStartLogTime;
        }

        public long ToLogSpace(long time)
        {
            return time + FightStartLogTime;
        }

        // Setters
        public void SetCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (_isCM == -1)
            {
                _isCM = Logic.IsCM(combatData, agentData, fightData);
            }
        }

        public void SetSuccess(bool success, long fightEndLogTime)
        {
            Success = success;
            FightEndLogTime = fightEndLogTime;
        }

        public void OverrideStart(long fightStartLogTime)
        {
            FightStartLogTime = fightStartLogTime;
        }
    }
}

