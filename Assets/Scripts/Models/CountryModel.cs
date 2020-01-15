using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountryModel
{
    [JsonProperty("name")]
    public string Name;

    [JsonProperty("short_name")]
    public string ShortName;

    [JsonProperty("flag_resx")]
    public string FlagResource;

    [JsonProperty("intelligence")]
    public int Intelligence;

    [JsonProperty("power")]
    public int Power;

    [JsonProperty("speed")]
    public int Speed;

    [JsonIgnore]
    public Texture Flag
    {
        get
        {
            return Resources.Load<Texture>(FlagResource);
        }
    }

    [JsonIgnore]
    public float RawInt
    {
        get
        {
            return (float)Intelligence / 2f;
        }
    }

    [JsonIgnore]
    public float RawPower
    {
        get
        {
            
            return (float)Power * 0.08f;
        }
    }

    [JsonIgnore]
    public float RawSpeed
    {
        get
        {
            return (float)Speed * 0.04f;
        }
    }

    public static List<CountryModel> GetCountries()
    {
        InitializeCountries();
        return JsonConvert.DeserializeObject<List<CountryModel>>(PlayerPrefs.GetString("countries"));
    }

    private static void InitializeCountries()
    {
        if (!PlayerPrefs.HasKey("countries"))
        {
            List<CountryModel> countryModels = new List<CountryModel>
            {
                new CountryModel
                {
                    Name = "Philippines",
                    ShortName = "PH",
                    FlagResource = "ph_flag",
                    Intelligence = 75,
                    Power = 85,
                    Speed = 80
                },
                new CountryModel
                {
                    Name = "Korea",
                    ShortName = "KOR",
                    FlagResource = "kor_flag",
                    Intelligence = 80,
                    Power = 80,
                    Speed = 85
                },
                new CountryModel
                {
                    Name = "Japan",
                    ShortName = "JPN",
                    FlagResource = "jpn_flag",
                    Intelligence = 85,
                    Power = 80,
                    Speed = 82
                }
            };

            string jsonString = JsonConvert.SerializeObject(countryModels);
            Debug.Log(jsonString);
            PlayerPrefs.SetString("countries", jsonString);
        }
    }
}

