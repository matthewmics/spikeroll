using Assets.Scripts.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    public class VSAllCountriesModel
    {
        private const string PREF_KEY = "vs_all_countries";

        [JsonProperty("current_level")]
        public int CurrentLevel { get; set; }

        [JsonProperty("order_of_countries")]
        public List<string> OrderOfCountries { get; set; } 

        [JsonProperty("allocated_int")]
        public int AllocatedInt { get; set; }

        [JsonProperty("allocated_power")]
        public int AllocatedPower { get; set; }

        [JsonProperty("allocated_speed")]
        public int AllocatedSpeed { get; set; }

        [JsonProperty("remaining_points")]
        public int RemainingPoints { get; set; }

        public static bool HasCurrentGame()
        {
            return PlayerPrefs.HasKey(PREF_KEY);
        }

        public static void SaveInstance(VSAllCountriesModel toSave)
        {
            string json = JsonConvert.SerializeObject(toSave);

            PlayerPrefs.SetString(PREF_KEY, json);

            PlayerPrefs.Save();
        }

        public static VSAllCountriesModel GetCurrentGame()
        {
            return JsonConvert.DeserializeObject<VSAllCountriesModel>(PlayerPrefs.GetString(PREF_KEY));
        }

        public static void StartNew()
        {
            VSAllCountriesModel vsm = new VSAllCountriesModel()
            {
                AllocatedInt = 75, AllocatedPower = 75, AllocatedSpeed = 75, CurrentLevel = 0 , RemainingPoints = 5
            };

            var countries = CountryModel.GetCountries();

            List<string> orderCountries = new List<string>();

            foreach(var i in countries)
            {
                orderCountries.Add(i.ShortName);
            }

            RandomUtil.Shuffle<string>(orderCountries);

            vsm.OrderOfCountries = orderCountries;

            SaveInstance(vsm);           


        }

    }