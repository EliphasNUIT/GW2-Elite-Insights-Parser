using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using GW2EIParser.Builders.JsonModels;
using GW2EIParser.EIData;
using GW2EIParser.Models;
using GW2EIParser.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static GW2EIParser.Builders.JsonModels.JsonBuffDamageModifierData;
using static GW2EIParser.Builders.JsonModels.JsonNPCBuffs;
using static GW2EIParser.Builders.JsonModels.JsonPlayerBuffsGeneration;
using static GW2EIParser.Builders.JsonModels.JsonPlayerBuffsUptime;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Builders
{
    public class RawFormatBuilder
    {
        private readonly ParsedLog _log;
        private readonly List<PhaseData> _phases;
        public JsonLog JsonLog { get; }
        //
        private readonly Dictionary<string, JsonLog.BuffDesc> _buffDesc = new Dictionary<string, JsonLog.BuffDesc>();
        private readonly Dictionary<string, JsonLog.DamageModDesc> _damageModDesc = new Dictionary<string, JsonLog.DamageModDesc>();
        private readonly Dictionary<string, HashSet<long>> _personalBuffs = new Dictionary<string, HashSet<long>>();

        public RawFormatBuilder(ParsedLog log, string[] uploadString)
        {
            JsonLog = new JsonLog(log, uploadString);
        }

        public void CreateJSON(StreamWriter sw)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            JsonTextWriter writer = new JsonTextWriter(sw)
            {
                Formatting = Properties.Settings.Default.IndentJSON ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None
            };
            serializer.Serialize(writer, JsonLog);
            writer.Close();
        }

        public void CreateXML(StreamWriter sw)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            Dictionary<string, JsonLog> root = new Dictionary<string, JsonLog>()
            {
                {"log", JsonLog }
            };
            string json = JsonConvert.SerializeObject(root, settings);

            XmlDocument xml = JsonConvert.DeserializeXmlNode(json);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(sw)
            {
                Formatting = Properties.Settings.Default.IndentXML ? System.Xml.Formatting.Indented : System.Xml.Formatting.None
            };

            xml.WriteTo(xmlTextWriter);
            xmlTextWriter.Close();
        }

        private JsonDPS[][] BuildDPSTarget(Player p)
        {
            JsonDPS[][] res = new JsonDPS[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (Target tar in _log.FightData.Logic.Targets)
            {
                res[i++] = p.GetDPS(_log, tar).Select(x => new JsonDPS(x)).ToArray();
            }
            return res;
        }

        private JsonStats[][] BuildStatsTarget(Player p)
        {
            JsonStats[][] res = new JsonStats[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (Target tar in _log.FightData.Logic.Targets)
            {
                res[i++] = p.GetStats(_log, tar).Select(x => new JsonStats(x)).ToArray();
            }
            return res;
        }

        private List<JsonBuffDamageModifierData> BuildDamageModifiers(Dictionary<string, List<Statistics.DamageModifierData>> extra)
        {
            Dictionary<int, List<JsonBuffDamageModifierItem>> dict = new Dictionary<int, List<JsonBuffDamageModifierItem>>();
            foreach (string key in extra.Keys)
            {
                int iKey = key.GetHashCode();
                string nKey = "d" + iKey;
                if (!_damageModDesc.ContainsKey(nKey))
                {
                    _damageModDesc[nKey] = new JsonLog.DamageModDesc(_log.DamageModifiers.DamageModifiersByName[key]);
                }
                dict[iKey] = extra[key].Select(x => new JsonBuffDamageModifierItem(x)).ToList();
            }
            List<JsonBuffDamageModifierData> res = new List<JsonBuffDamageModifierData>();
            foreach (var pair in dict)
            {
                res.Add(new JsonBuffDamageModifierData()
                {
                    Id = pair.Key,
                    DamageModifiers = pair.Value
                });
            }
            return res;
        }

        private List<JsonBuffDamageModifierData>[] BuildDamageModifiersTarget(Player p)
        {
            List<JsonBuffDamageModifierData>[] res = new List<JsonBuffDamageModifierData>[_log.FightData.Logic.Targets.Count];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target tar = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageModifiers(p.GetDamageModifierData(_log, tar));
            }
            return res;
        }

        private static List<int[]> BuildBuffStates(BuffsGraphModel bgm)
        {
            if (bgm == null || bgm.BoonChart.Count == 0)
            {
                return null;
            }
            List<int[]> res = bgm.BoonChart.Select(x => new int[2] { (int)x.Start, x.Value }).ToList();
            return res.Count > 0 ? res : null;
        }

        private List<JsonNPCBuffs> BuildTargetBuffs(List<Dictionary<long, Statistics.FinalTargetBuffs>> statBoons, Target target)
        {
            var boons = new List<JsonNPCBuffs>();

            foreach (var pair in statBoons[0])
            {
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(_log.Buffs.BuffsByIds[pair.Key]);
                }
                List<JsonTargetBuffsData> data = new List<JsonTargetBuffsData>();
                for (int i = 0; i < _phases.Count; i++)
                {
                    JsonTargetBuffsData value = new JsonTargetBuffsData(statBoons[i][pair.Key]);
                    data.Add(value);
                }
                JsonNPCBuffs jsonBuffs = new JsonNPCBuffs()
                {
                    States = BuildBuffStates(target.GetBuffGraphs(_log)[pair.Key]),
                    BuffData = data,
                    Id = pair.Key
                };
                boons.Add(jsonBuffs);
            }

            return boons;
        }

        private List<JsonPlayerBuffsGeneration> BuildPlayerBuffGenerations(List<Dictionary<long, Statistics.FinalBuffs>> statUptimes)
        {
            var uptimes = new List<JsonPlayerBuffsGeneration>();
            foreach (var pair in statUptimes[0])
            {
                Buff buff = _log.Buffs.BuffsByIds[pair.Key];
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(buff);
                }
                List<JsonBuffsGenerationData> data = new List<JsonBuffsGenerationData>();
                for (int i = 0; i < _phases.Count; i++)
                {
                    data.Add(new JsonBuffsGenerationData(statUptimes[i][pair.Key]));
                }
                JsonPlayerBuffsGeneration jsonBuffs = new JsonPlayerBuffsGeneration()
                {
                    BuffData = data,
                    Id = pair.Key
                };
                uptimes.Add(jsonBuffs);
            }

            if (!uptimes.Any()) return null;

            return uptimes;
        }

        private List<JsonPlayerBuffsUptime> BuildPlayerBuffUptimes(List<Dictionary<long, Statistics.FinalBuffs>> statUptimes, Player player)
        {
            var uptimes = new List<JsonPlayerBuffsUptime>();
            foreach (var pair in statUptimes[0])
            {
                Buff buff = _log.Buffs.BuffsByIds[pair.Key];
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(buff);
                }
                if (buff.Nature == Buff.BuffNature.GraphOnlyBuff && buff.Source == Buff.ProfToEnum(player.Prof))
                {
                    if (player.GetBuffDistribution(_log, 0).GetUptime(pair.Key) > 0)
                    {
                        if (_personalBuffs.TryGetValue(player.Prof, out var list) && !list.Contains(pair.Key))
                        {
                            list.Add(pair.Key);
                        }
                        else
                        {
                            _personalBuffs[player.Prof] = new HashSet<long>()
                                {
                                    pair.Key
                                };
                        }
                    }
                }
                List<JsonBuffsUptimeData> data = new List<JsonBuffsUptimeData>();
                for (int i = 0; i < _phases.Count; i++)
                {
                    data.Add(new JsonBuffsUptimeData(statUptimes[i][pair.Key]));
                }
                JsonPlayerBuffsUptime jsonBuffs = new JsonPlayerBuffsUptime()
                {
                    States = BuildBuffStates(player.GetBuffGraphs(_log)[pair.Key]),
                    BuffData = data,
                    Id = pair.Key
                };
                uptimes.Add(jsonBuffs);
            }

            if (!uptimes.Any()) return null;

            return uptimes;
        }
    }
}