using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountryModel
{
    [JsonProperty("is_locked")]
    public bool IsLocked;

    [JsonProperty("unlock_cost")]
    public int UnlockCost;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("short_name")]
    public string ShortName;

    [JsonProperty("flag_resx")]
    public string FlagResource;

    [JsonProperty("animator_location")]
    public string AnimatorLocation;

    [JsonProperty("intelligence")]
    public int Intelligence;

    [JsonProperty("power")]
    public int Power;

    [JsonProperty("speed")]
    public int Speed;

    [JsonIgnore]
    public RuntimeAnimatorController ControllerBottom1
    {
        get
        {
            return Resources.Load<RuntimeAnimatorController>(AnimatorLocation + "btm1");
        }
    }
    [JsonIgnore]
    public RuntimeAnimatorController ControllerBottom2
    {
        get
        {
            return Resources.Load<RuntimeAnimatorController>(AnimatorLocation + "btm2");
        }
    }
    [JsonIgnore]
    public RuntimeAnimatorController ControllerTop1
    {
        get
        {
            return Resources.Load<RuntimeAnimatorController>(AnimatorLocation + "top1");
        }
    }
    [JsonIgnore]
    public RuntimeAnimatorController ControllerTop2
    {
        get
        {
            return Resources.Load<RuntimeAnimatorController>(AnimatorLocation + "top2");
        }
    }

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
            int tint = Intelligence - 50;
            return (float)tint * 2f;
        }
    }

    [JsonIgnore]
    public float RawPower
    {
        get
        {
            int tpower = Power - 50;
            return (float)tpower * 0.13f;
           // return (float)Power * 0.06f;
        }
    }

    [JsonIgnore]
    public float RawSpeed
    {
        get
        {
            int tspeed = Speed - 50;
            return (float)tspeed * 0.1f;
           // return (float)Speed * 0.04f;
        }
    }

    public static CountryModel GetCountryByShortName(string shortname)
    {
        List<CountryModel> countries = GetCountries();
        foreach(CountryModel country in countries)
        {
            if (country.ShortName.Equals(shortname))
            {
                return country;
            }
        }

        return null;
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
                    IsLocked = false,
                    UnlockCost = 0,
                    ShortName = "PH",
                    FlagResource = "ph_flag",
                    AnimatorLocation = "Controllers/PH/ph",
                    Intelligence = 75,
                    Power = 85,
                    Speed = 80
                },
                new CountryModel
                {
                    Name = "Japan",
                    IsLocked = true,
                    UnlockCost = 100,
                    ShortName = "JPN",
                    FlagResource = "jpn_flag",
                    AnimatorLocation = "Controllers/JPN/jpn",
                    Intelligence = 85,
                    Power = 80,
                    Speed = 82
                },
                new CountryModel
                {
                    Name = "Korea",
                    IsLocked = true,
                    UnlockCost = 200,
                    ShortName = "KOR",
                    FlagResource = "kor_flag",
                    AnimatorLocation = "Controllers/KOR/kor",
                    Intelligence = 80,
                    Power = 80,
                    Speed = 85
                },
                new CountryModel
                {
                    Name = "Thailand",

                    IsLocked = true,
                    UnlockCost = 200,
                    ShortName = "TH",
                    FlagResource = "th_flag",
                    AnimatorLocation = "Controllers/TH/th",
                    Intelligence = 75,
                    Power = 82,
                    Speed = 83
                },
                new CountryModel
                {
                    Name = "Malaysia",
                    IsLocked = true,
                    UnlockCost = 300,
                    ShortName = "MY",
                    FlagResource = "my_flag",
                    AnimatorLocation = "Controllers/My/my",
                    Intelligence = 78,
                    Power = 86,
                    Speed = 81
                }
            };

            string jsonString = JsonConvert.SerializeObject(countryModels);
            //Debug.Log(jsonString);
            PlayerPrefs.SetString("countries", jsonString);
            PlayerPrefs.Save();
        }
    }


    public static void OverwriteCountries(List<CountryModel>countryModels)
    {

        string jsonString = JsonConvert.SerializeObject(countryModels);
        //Debug.Log(jsonString);
        PlayerPrefs.SetString("countries", jsonString);
        PlayerPrefs.Save();
    }
}

