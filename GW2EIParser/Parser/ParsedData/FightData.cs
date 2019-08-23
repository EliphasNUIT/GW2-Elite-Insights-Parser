using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Logic;

namespace GW2EIParser.Parser.ParsedData
{
    public class FightData
    {
        // Fields
        private List<PhaseData> _phases = new List<PhaseData>();
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
            switch (ParseEnum.GetTargetIDS(id))
            {
                case ParseEnum.EvtcTargetIDS.ValeGuardian:
                    Logic = new ValeGuardian(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Gorseval:
                    Logic = new Gorseval(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Sabetha:
                    Logic = new Sabetha(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Slothasor:
                    Logic = new Slothasor(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Zane:
                case ParseEnum.EvtcTargetIDS.Berg:
                case ParseEnum.EvtcTargetIDS.Narella:
                    Logic = new BanditTrio(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Matthias:
                    Logic = new Matthias(id);
                    break;
                /*case ParseEnum.TargetIDS.Escort:
                    Logic = new Escort(id, agentData);
                    break;*/
                case ParseEnum.EvtcTargetIDS.KeepConstruct:
                    Logic = new KeepConstruct(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Xera:
                    Logic = new Xera(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Cairn:
                    Logic = new Cairn(id);
                    break;
                case ParseEnum.EvtcTargetIDS.MursaatOverseer:
                    Logic = new MursaatOverseer(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Samarog:
                    Logic = new Samarog(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Deimos:
                    Logic = new Deimos(id);
                    break;
                case ParseEnum.EvtcTargetIDS.SoullessHorror:
                    Logic = new SoullessHorror(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Desmina:
                    Logic = new River(id);
                    break;
                case ParseEnum.EvtcTargetIDS.BrokenKing:
                    Logic = new BrokenKing(id);
                    break;
                case ParseEnum.EvtcTargetIDS.SoulEater:
                    Logic = new EaterOfSouls(id);
                    break;
                case ParseEnum.EvtcTargetIDS.EyeOfFate:
                case ParseEnum.EvtcTargetIDS.EyeOfJudgement:
                    Logic = new DarkMaze(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Dhuum:
                    // some eyes log are registered as Dhuum
                    if (agentData.GetAgentsByID((ushort)ParseEnum.EvtcTargetIDS.EyeOfFate).Count > 0 ||
                        agentData.GetAgentsByID((ushort)ParseEnum.EvtcTargetIDS.EyeOfJudgement).Count > 0)
                    {
                        ID = (ushort)ParseEnum.EvtcTargetIDS.EyeOfFate;
                        Logic = new DarkMaze((ushort)ParseEnum.EvtcTargetIDS.EyeOfFate);
                        break;
                    }
                    Logic = new Dhuum(id);
                    break;
                case ParseEnum.EvtcTargetIDS.ConjuredAmalgamate:
                    Logic = new ConjuredAmalgamate(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Kenut:
                case ParseEnum.EvtcTargetIDS.Nikare:
                    Logic = new TwinLargos(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Qadim:
                    Logic = new Qadim(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Freezie:
                    Logic = new Freezie(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Adina:
                    Logic = new Adina(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Sabir:
                    Logic = new Sabir(id);
                    break;
                case ParseEnum.EvtcTargetIDS.PeerlessQadim:
                    Logic = new PeerlessQadim(id);
                    break;
                case ParseEnum.EvtcTargetIDS.MAMA:
                    Logic = new MAMA(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Siax:
                    Logic = new Siax(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Ensolyss:
                    Logic = new Ensolyss(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Skorvald:
                    Logic = new Skorvald(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Artsariiv:
                    Logic = new Artsariiv(id);
                    break;
                case ParseEnum.EvtcTargetIDS.Arkk:
                    Logic = new Arkk(id);
                    break;
                case ParseEnum.EvtcTargetIDS.WorldVersusWorld:
                    Logic = new WvWFight(id);
                    break;
                case ParseEnum.EvtcTargetIDS.MassiveGolem:
                case ParseEnum.EvtcTargetIDS.AvgGolem:
                case ParseEnum.EvtcTargetIDS.LGolem:
                case ParseEnum.EvtcTargetIDS.MedGolem:
                case ParseEnum.EvtcTargetIDS.StdGolem:
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

            if (_phases.Count == 0)
            {
                long fightDuration = log.FightData.FightDuration;
                _phases = log.FightData.Logic.GetPhases(log, _requirePhases);
            }
            _phases.RemoveAll(x => x.DurationInMS <= 1000);
            return _phases;
        }

        public List<Target> GetMainTargets(ParsedLog log)
        {
            if (_phases.Count == 0)
            {
                long fightDuration = log.FightData.FightDuration;
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