using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using GW2EIParser.Controllers.GW2API;

namespace GW2EIParser.Controllers
{
    public static class GW2APIController
    {
        private static HttpClient APIClient { get; set; }

        private static void GetAPIClient()
        {
            if (APIClient == null)
            {
                APIClient = new HttpClient
                {
                    BaseAddress = new Uri("https://api.guildwars2.com")
                };
                APIClient.DefaultRequestHeaders.Accept.Clear();
                APIClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            return;
        }

        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

        //----------------------------------------------------------------------------- SKILLS

        private class SkillList
        {
            public SkillList() { Items = new List<GW2APISkill>(); }
            public List<GW2APISkill> Items { get; set; }
        }

        private static SkillList _listOfSkills = new SkillList();

        public static GW2APISkill GetSkill(long id)
        {
            GW2APISkill skill = GetSkillList().Items.FirstOrDefault(x => x.Id == id);
            return skill;
        }
        private static List<GW2APISkill> GetListGW2APISkills()
        {
            var skill_L = new List<GW2APISkill>();
            bool maxPageSizeReached = false;
            int page = 0;
            int pagesize = 200;
            while (!maxPageSizeReached)
            {
                string path = "/v2/skills?page=" + page + "&page_size=" + pagesize;
                HttpResponseMessage response = APIClient.GetAsync(new Uri(path, UriKind.Relative)).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    GW2APISkill[] responseArray = JsonSerializer.Deserialize<GW2APISkill[]>(data, _options);
                    skill_L.AddRange(responseArray);
                }
                else
                {
                    maxPageSizeReached = true;
                }
                page++;
            }

            return skill_L;
        }
        private static SkillList GetSkillList()
        {
            if (_listOfSkills.Items.Count == 0)
            {
                SetSkillList();
            }
            return _listOfSkills;
        }
        public static List<int> WriteSkillListToFile()
        {
            FileStream fcreate = File.Open(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SkillList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            _listOfSkills = new SkillList();
            HttpResponseMessage response = APIClient.GetAsync(new Uri("/v2/skills", UriKind.Relative)).Result;
            var failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list           
                _listOfSkills.Items.AddRange(GetListGW2APISkills());
                var writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SkillList.json");
                writer.Write(JsonSerializer.Serialize(_listOfSkills.Items, _options));
                writer.Close();

            }
            return failedList;
        }
        private static void SetSkillList()
        {

            if (_listOfSkills.Items.Count == 0)
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/SkillList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading Skilllist");
                        using var reader = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/SkillList.json");
                        _listOfSkills.Items = JsonSerializer.Deserialize<List<GW2APISkill>>(reader.ReadToEnd(), _options);
                        reader.Close();
                    }
                }
                if (_listOfSkills.Items.Count == 0)
                {
                    WriteSkillListToFile();
                }
            }
            return;
        }
        //----------------------------------------------------------------------------- SPECS
        private class SpecList
        {
            public SpecList() { Items = new List<GW2APISpec>(); }

            public List<GW2APISpec> Items { get; set; }
        }

        private static SpecList _listofSpecs = new SpecList();

        public static GW2APISpec GetSpec(int id)
        {
            GW2APISpec spec = GetSpecList().Items.FirstOrDefault(x => x.Id == id);

            return spec;
        }
        private static GW2APISpec GetGW2APISpec(string path)
        {
            GW2APISpec spec = null;
            //path = "/v2/specializations/" + isElite
            HttpResponseMessage response = APIClient.GetAsync(new Uri(path, UriKind.Relative)).Result;
            if (response.IsSuccessStatusCode)
            {
                spec = JsonSerializer.Deserialize<GW2APISpec>(response.Content.ReadAsStringAsync().Result, _options);

            }
            return spec;
        }

        private static SpecList GetSpecList()
        {
            if (_listofSpecs.Items.Count == 0)
            {
                SetSpecList();
            }
            return _listofSpecs;
        }
        public static List<int> WriteSpecListToFile()
        {
            FileStream fcreate = File.Open(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SpecList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            _listofSpecs = new SpecList();
            HttpResponseMessage response = APIClient.GetAsync(new Uri("/v2/specializations", UriKind.Relative)).Result;
            int[] idArray;
            var failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list
                idArray = JsonSerializer.Deserialize<int[]>(response.Content.ReadAsStringAsync().Result, _options);

                foreach (int id in idArray)
                {
                    var curSpec = new GW2APISpec();
                    curSpec = GetGW2APISpec("/v2/specializations/" + id);
                    if (curSpec != null)
                    {

                        _listofSpecs.Items.Add(curSpec);

                    }
                    else
                    {
                        Console.WriteLine("Failed to get response");//fail to retrieve
                        failedList.Add(id);
                    }

                }
                var writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SpecList.json");
                writer.Write(JsonSerializer.Serialize(_listofSpecs.Items, _options));
                writer.Close();

            }
            return failedList;
        }

        private static void SetSpecList()
        {

            if (_listofSpecs.Items.Count == 0)
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/SpecList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading SpecList");
                        using var reader = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/SpecList.json");
                        _listofSpecs.Items = JsonSerializer.Deserialize<List<GW2APISpec>>(reader.ReadToEnd(), _options);
                        reader.Close();
                    }
                }
                if (_listofSpecs.Items.Count == 0)//if nothing in file or fail write new file
                {
                    WriteSpecListToFile();
                }

            }
            return;
        }

        public static string GetAgentProfString(uint prof, uint elite)
        {
            // non player
            if (elite == 0xFFFFFFFF)
            {
                if ((prof & 0xffff0000) == 0xffff0000)
                {
                    return "GDG";
                }
                else
                {
                    return "NPC";
                }
            }
            // base profession
            else if (elite == 0)
            {
                switch (prof)
                {
                    case 1:
                        return "Guardian";
                    case 2:
                        return "Warrior";
                    case 3:
                        return "Engineer";
                    case 4:
                        return "Ranger";
                    case 5:
                        return "Thief";
                    case 6:
                        return "Elementalist";
                    case 7:
                        return "Mesmer";
                    case 8:
                        return "Necromancer";
                    case 9:
                        return "Revenant";
                }
            }
            // old elite
            else if (elite == 1)
            {
                switch (prof)
                {
                    case 1:
                        return "Dragonhunter";
                    case 2:
                        return "Berserker";
                    case 3:
                        return "Scrapper";
                    case 4:
                        return "Druid";
                    case 5:
                        return "Daredevil";
                    case 6:
                        return "Tempest";
                    case 7:
                        return "Chronomancer";
                    case 8:
                        return "Reaper";
                    case 9:
                        return "Herald";
                }

            }
            // new way
            else
            {
                GW2APISpec spec = GetSpec((int)elite);
                if (spec.Elite)
                {
                    return spec.Name;
                }
                else
                {
                    return spec.Profession;
                }
            }
            throw new InvalidDataException("Unknown profession");
        }

        //----------------------------------------------------------------------------- TRAITS

        private class TraitList
        {
            public TraitList() { Items = new List<GW2APITrait>(); }
            public List<GW2APITrait> Items { get; set; }
        }

        private static TraitList _listOfTraits = new TraitList();

        public static GW2APITrait GetTrait(long id)
        {
            GW2APITrait trait = GetTraitList().Items.FirstOrDefault(x => x.Id == id);
            return trait;
        }
        private static List<GW2APITrait> GetListGW2APITraits()
        {
            var trait_L = new List<GW2APITrait>();
            bool maxPageSizeReached = false;
            int page = 0;
            int pagesize = 200;
            while (!maxPageSizeReached)
            {
                string path = "/v2/traits?page=" + page + "&page_size=" + pagesize;
                HttpResponseMessage response = APIClient.GetAsync(new Uri(path, UriKind.Relative)).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    GW2APITrait[] responseArray = JsonSerializer.Deserialize<GW2APITrait[]>(data, _options);
                    trait_L.AddRange(responseArray);
                }
                else
                {
                    maxPageSizeReached = true;
                }
                page++;
            }

            return trait_L;
        }
        private static TraitList GetTraitList()
        {
            if (_listOfTraits.Items.Count == 0)
            {
                SetTraitList();
            }
            return _listOfTraits;
        }
        public static List<int> WriteTraitListToFile()
        {
            FileStream fcreate = File.Open(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/TraitList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            _listOfTraits = new TraitList();
            HttpResponseMessage response = APIClient.GetAsync(new Uri("/v2/traits", UriKind.Relative)).Result;
            var failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list           
                _listOfTraits.Items.AddRange(GetListGW2APITraits());
                var writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/TraitList.json");
                writer.Write(JsonSerializer.Serialize(_listOfTraits.Items, _options));
                writer.Close();

            }
            return failedList;
        }
        private static void SetTraitList()
        {

            if (_listOfTraits.Items.Count == 0)
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/TraitList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading Traitlist");
                        using var reader = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/TraitList.json");
                        _listOfTraits.Items = JsonSerializer.Deserialize< List<GW2APITrait>>(reader.ReadToEnd(), _options);
                        reader.Close();
                    }
                }
                if (_listOfTraits.Items.Count == 0)
                {
                    WriteTraitListToFile();
                }
            }
            return;
        }
    }
}
