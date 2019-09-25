using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml;
using GW2EIParser.Builders.JsonModels;
using GW2EIParser.Parser;

namespace GW2EIParser.Builders
{
    public class JsonBuilder
    {
        public JsonLog JsonLog { get; }
        //

        public JsonBuilder(ParsedLog log, string[] uploadString)
        {
            JsonLog = new JsonLog(log, uploadString);
        }

        public void CreateJSON(StreamWriter sw)
        {
            var settings = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = Properties.Settings.Default.IndentJSON
            };
            sw.Write(JsonSerializer.Serialize(JsonLog, settings));
        }

        /*public void CreateXML(StreamWriter sw)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            var root = new Dictionary<string, JsonLog>()
            {
                {"log", JsonLog }
            };
            string json = JsonConvert.SerializeObject(root, settings);

            XmlDocument xml = JsonConvert.DeserializeXmlNode(json);
            var xmlTextWriter = new XmlTextWriter(sw)
            {
                Formatting = Properties.Settings.Default.IndentXML ? System.Xml.Formatting.Indented : System.Xml.Formatting.None
            };

            xml.WriteTo(xmlTextWriter);
            xmlTextWriter.Close();
        }*/
    }
}
