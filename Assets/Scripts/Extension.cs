using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TitleScreenTypes
{
    Title,
    Pause
}

public enum TitleScreenOptions
{
    Start,
    Option,
    Quit
}

public enum OptionScreenOptions
{
    Music,
    SFX,
    OK
}

static class Settings
{
    public static float music = 50;
    public static float SFX = 50;
}

static class Internals
{
    public static int winTime = 0;
    public static bool dialogOpened = false;
    public static int gems = 0;
    public static float zombieHPScaler = 1.0f;
}

static class Constants
{
    public static Vector3[] titleScreenOptionPositions = { new Vector3(-163, 53,0),
                                                    new Vector3(-163, -3,0),
                                                    new Vector3(-163, -59,0)};

    public static Vector2 volumeSliderXRange = new Vector2(-158, 358);
}

static class Extension
{
    public static T Next<T>(this T option) where T : struct, Enum
    {
        int newValue = (int)(object)option + 1;
        if (newValue >= Enum.GetNames(typeof(T)).Length) { newValue = 0; }
        return (T)(object)newValue;
    }

    public static T Previous<T>(this T option) where T : struct, Enum
    {
        int newValue = (int)(object)option - 1;
        if (newValue < 0) { newValue = Enum.GetNames(typeof(T)).Length - 1; }
        return (T)(object)newValue;
    }

    public static float Bounds(this float num, float min, float max)
    {
        float output = num;
        if (num < min)
        {
            output = min;
        }
        if (num > max)
        {
            output = max;
        }
        return output;
    }
}
