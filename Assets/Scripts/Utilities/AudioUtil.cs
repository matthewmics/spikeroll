using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioUtil
{
    private static void InitializeAudio()
    {
        if (!PlayerPrefs.HasKey("audio_bgm"))
        {
            PlayerPrefs.SetFloat("audio_bgm", 1.0f);
        }
        if (!PlayerPrefs.HasKey("audio_sfx"))
        {
            PlayerPrefs.SetFloat("audio_sfx", 1.0f);
        }
    }

    public static float GetBgm()
    {
        InitializeAudio();
        return PlayerPrefs.GetFloat("audio_bgm");
    }
    public static float GetSfx()
    {
        InitializeAudio();
        return PlayerPrefs.GetFloat("audio_sfx");
    }

    public static void SetBgm(float val)
    {
        InitializeAudio();
        PlayerPrefs.SetFloat("audio_bgm", val);
    }


    public static void SetSfx(float val)
    {
        InitializeAudio();
        PlayerPrefs.SetFloat("audio_sfx", val);
    }


}