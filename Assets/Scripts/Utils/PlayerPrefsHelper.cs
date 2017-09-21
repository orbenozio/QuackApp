using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsHelper
{
    
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static string GetString(string key, string defaultValue = default(string))
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

}

public static class PlayerPrefsConsts
{
    public const string PP_LOGIN_METHOD = "login_method";

    public const string PP_FACEBOOK_LOGIN = "facebook";
}

