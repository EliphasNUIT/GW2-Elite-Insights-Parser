using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using GW2EIParser.Builders.JsonModels;
using GW2EIParser.Parser;
using Newtonsoft.Json;

namespace GW2EIParser.Builders
{
    public class HTMLBuilder
    {
        private readonly string _scriptVersion;
        private readonly int _scriptVersionRev;

        private readonly bool _cr;

        private readonly JsonLog _jsonLog;
        // https://point2blog.wordpress.com/2012/12/26/compressdecompress-a-string-in-c/
        private static string CompressAndBase64(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                msi.CopyTo(gs);
            }
            return Convert.ToBase64String(mso.ToArray());
        }

        public HTMLBuilder(JsonLog jsonLog, ParsedLog log)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _scriptVersion = version.Major + "." + version.Minor;
#if !DEBUG
            _scriptVersion += "." + version.Build;
#endif
            _scriptVersionRev = version.Revision;

            _jsonLog = jsonLog;

            _cr = Properties.Settings.Default.ParseCombatReplay && log.CanCombatReplay;
        }

        public void CreateHTML(StreamWriter sw, string path)
        {
            string html = Properties.Resources.template_html;

            html = html.Replace("${bootstrapTheme}", !Properties.Settings.Default.LightTheme ? "slate" : "yeti");

            html = html.Replace("<!--${Css}-->", BuildCss(path));
            html = html.Replace("<!--${Js}-->", BuildEIJs(path));
            html = html.Replace("<!--${JsCRLink}-->", BuildCRLinkJs(path));

            html = html.Replace("'${logDataJson}'", "'" + CompressAndBase64(ToJson(_jsonLog)) + "'");
#if DEBUG
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.js\"></script>");
#else
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.min.js\"></script>");
#endif

            html = html.Replace("<!--${CombatReplayScript}-->", BuildCombatReplayScript(path));
            sw.Write(html);
            return;
        }

        private static string BuildCombatReplayScript(string path)
        {
            //if (!_cr)
            //{
            return "";
            //}
            /*string scriptContent = Properties.Resources.combatreplay_js;
            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".debug.js";
#else
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".js";
#endif
                string jsPath = Path.Combine(path, jsFileName);
                try
                {
                    using var fs = new FileStream(jsPath, FileMode.Create, FileAccess.Write);
                    using var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8);
                    scriptWriter.Write(scriptContent);
                }
                catch (IOException)
                {
                }
                string content = "<script src=\"./" + jsFileName + "?version=" + _scriptVersionRev + "\"></script>\n";
                return content;
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }*/
        }

        private static string BuildTemplates(string script)
        {
            string tmplScript = script;
            var templates = new Dictionary<string, string>()
            {
                /*{"${tmplBuffStats}",Properties.Resources.tmplBuffStats },
                {"${tmplBuffStatsTarget}",Properties.Resources.tmplBuffStatsTarget },
                {"${tmplBuffTable}",Properties.Resources.tmplBuffTable },
                {"${tmplDamageDistPlayer}",Properties.Resources.tmplDamageDistPlayer },
                {"${tmplDamageDistTable}",Properties.Resources.tmplDamageDistTable },
                {"${tmplDamageDistTarget}",Properties.Resources.tmplDamageDistTarget },
                {"${tmplDamageModifierTable}",Properties.Resources.tmplDamageModifierTable },
                {"${tmplDamageModifierStats}",Properties.Resources.tmplDamageModifierStats },
                {"${tmplDamageModifierPersStats}",Properties.Resources.tmplDamageModifierPersStats },
                {"${tmplDamageTable}",Properties.Resources.tmplDamageTable },
                {"${tmplDamageTaken}",Properties.Resources.tmplDamageTaken },
                {"${tmplDeathRecap}",Properties.Resources.tmplDeathRecap },
                {"${tmplDefenseTable}",Properties.Resources.tmplDefenseTable },
                {"${tmplEncounter}",Properties.Resources.tmplEncounter },
                {"${tmplFood}",Properties.Resources.tmplFood },
                {"${tmplGameplayTable}",Properties.Resources.tmplGameplayTable },
                {"${tmplGeneralLayout}",Properties.Resources.tmplGeneralLayout },
                {"${tmplMechanicsTable}",Properties.Resources.tmplMechanicsTable },
                {"${tmplPersonalBuffTable}",Properties.Resources.tmplPersonalBuffTable },
                {"${tmplPhase}",Properties.Resources.tmplPhase },
                {"${tmplPlayers}",Properties.Resources.tmplPlayers },
                {"${tmplPlayerStats}",Properties.Resources.tmplPlayerStats },
                {"${tmplPlayerTab}",Properties.Resources.tmplPlayerTab },
                {"${tmplSimpleRotation}",Properties.Resources.tmplSimpleRotation },
                {"${tmplSupportTable}",Properties.Resources.tmplSupportTable },
                {"${tmplTargets}",Properties.Resources.tmplTargets },
                {"${tmplTargetStats}",Properties.Resources.tmplTargetStats },
                {"${tmplTargetTab}",Properties.Resources.tmplTargetTab },
                {"${tmplDPSGraph}",Properties.Resources.tmplDPSGraph },
                {"${tmplGraphStats}",Properties.Resources.tmplGraphStats },
                {"${tmplPlayerTabGraph}",Properties.Resources.tmplPlayerTabGraph },
                {"${tmplRotationLegend}",Properties.Resources.tmplRotationLegend },
                {"${tmplTargetTabGraph}",Properties.Resources.tmplTargetTabGraph },
                {"${tmplTargetData}",Properties.Resources.tmplTargetData },*/
            };
            foreach (KeyValuePair<string, string> entry in templates)
            {
#if DEBUG
                tmplScript = tmplScript.Replace(entry.Key, entry.Value);
#else
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
#endif
            }
            return tmplScript;
        }

        private static string BuildCRTemplates(string script)
        {
            string tmplScript = script;
            var CRtemplates = new Dictionary<string, string>()
            {
                /*{"${tmplCombatReplayDamageData}", Properties.Resources.tmplCombatReplayDamageData },
                {"${tmplCombatReplayStatusData}", Properties.Resources.tmplCombatReplayStatusData },
                {"${tmplCombatReplayDamageTable}", Properties.Resources.tmplCombatReplayDamageTable },
                {"${tmplCombatReplayActorBuffStats}", Properties.Resources.tmplCombatReplayActorBuffStats },
                {"${tmplCombatReplayPlayerStats}", Properties.Resources.tmplCombatReplayPlayerStats },
                {"${tmplCombatReplayPlayerStatus}", Properties.Resources.tmplCombatReplayPlayerStatus },
                {"${tmplCombatReplayActorRotation}", Properties.Resources.tmplCombatReplayActorRotation },
                {"${tmplCombatReplayTargetStats}", Properties.Resources.tmplCombatReplayTargetStats },
                {"${tmplCombatReplayTargetStatus}", Properties.Resources.tmplCombatReplayTargetStatus },
                {"${tmplCombatReplayTargetsStats}", Properties.Resources.tmplCombatReplayTargetsStats },
                {"${tmplCombatReplayPlayersStats}", Properties.Resources.tmplCombatReplayPlayersStats },
                {"${tmplCombatReplayUI}", Properties.Resources.tmplCombatReplayUI },
                {"${tmplCombatReplayPlayerSelect}", Properties.Resources.tmplCombatReplayPlayerSelect },
                {"${tmplCombatReplayRangeSelect}", Properties.Resources.tmplCombatReplayRangeSelect },
                {"${tmplCombatReplayAnimationControl}", Properties.Resources.tmplCombatReplayAnimationControl },*/
            };
            foreach (KeyValuePair<string, string> entry in CRtemplates)
            {
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
            }
            return tmplScript;
        }

        private static string BuildCss(string path)
        {
            return "";
            /*string scriptContent = Properties.Resources.ei_css;

            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string cssFilename = "EliteInsights-" + _scriptVersion + ".debug.css";
#else
                string cssFilename = "EliteInsights-" + _scriptVersion + ".css";
#endif
                string cssPath = Path.Combine(path, cssFilename);
                try
                {
                    using var fs = new FileStream(cssPath, FileMode.Create, FileAccess.Write);
                    using var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8);
                    scriptWriter.Write(scriptContent);
                }
                catch (IOException)
                {
                }
                return "<link rel=\"stylesheet\" type=\"text/css\" href=\"./" + cssFilename + "?version=" + _scriptVersionRev + "\">";
            }
            else
            {
                return "<style type=\"text/css\">\r\n" + scriptContent + "\r\n</style>";
            }*/
        }

        private static string BuildEIJs(string path)
        {
            return "";
            /*var orderedScripts = new List<string>()
            {
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            scriptContent = BuildTemplates(scriptContent);

            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".debug.js";
#else
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write);
                    using var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8);
                    scriptWriter.Write(scriptContent);
                }
                catch (IOException)
                {
                }
                return "<script src=\"./" + scriptFilename + "?version=" + _scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }*/
        }

        private static string BuildCRLinkJs(string path)
        {
            //if (!_cr)
            //{
            return "";
            //}
            /*var orderedScripts = new List<string>()
            {
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            scriptContent = BuildCRTemplates(scriptContent);

            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion + ".debug.js";
#else
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion + ".js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write);
                    using var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8);
                    scriptWriter.Write(scriptContent);
                }
                catch (IOException)
                {
                }
                return "<script src=\"./" + scriptFilename + "?version=" + _scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }*/
        }

        private static string ToJson(object value)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = GeneralHelper.ContractResolver,
                //DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}
