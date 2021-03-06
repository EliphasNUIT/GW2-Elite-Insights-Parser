﻿using System.IO;
using GW2EIParser.Builders.JsonModels;
using GW2EIParser.Parser;
using Newtonsoft.Json;

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
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                //DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = GeneralHelper.ContractResolver
            };
            var writer = new JsonTextWriter(sw)
            {
                Formatting = Properties.Settings.Default.IndentJSON ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None
            };
            serializer.Serialize(writer, JsonLog);
            writer.Close();
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

