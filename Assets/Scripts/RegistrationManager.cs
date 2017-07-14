using GooglePlayGames;
using System;
using UnityEngine;

public delegate void RegistrationDelegte(eRegistrationMethodType reg, bool success, eRegistrationResultType resultType, string message = "");

public static class RegistrationManager
{
    public static event RegistrationDelegte OnSignInEvent;
    public static event RegistrationDelegte OnSignOutEvent;

    #region Public Methods

    public static void ActivateServices()
    {
        PlayGamesPlatform.Activate();
    }

    public static void SignIn(eRegistrationMethodType registrationMethodType)
    {
        switch (registrationMethodType)
        {
            case eRegistrationMethodType.Google:
                {
                    signInGoogle();
                    break;
                }
            default:
                {
                    if (OnSignInEvent != null)
                    {
                        OnSignInEvent(registrationMethodType, false, eRegistrationResultType.UnkownService);
                        return;
                    }
                    break;
                }
        }        
    }

    public static void SignOut(eRegistrationMethodType registrationMethodType)
    {
        switch (registrationMethodType)
        {
            case eRegistrationMethodType.Google:
                {
                    signOutGoogle();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }


    #endregion

    #region Private Methods

    private static void signInGoogle()
    {
#if UNITY_EDITOR
        OnSignInEvent(eRegistrationMethodType.Google, true, eRegistrationResultType.LogInSuccess);
        return;
#endif

        if (Social.localUser.authenticated)
        {
            if (OnSignInEvent != null)
            {
                OnSignInEvent(eRegistrationMethodType.Google, false, eRegistrationResultType.AlreadyLoggedIn);
                return;
            }
        }

        Social.localUser.Authenticate((bool success, string msg) =>
        {
            if (success)
            {
                OnSignInEvent(eRegistrationMethodType.Google, success, eRegistrationResultType.LogInSuccess);

            }
            else
            {
                OnSignInEvent(eRegistrationMethodType.Google, success, eRegistrationResultType.LogInFail, msg);
            }
        });
    }

    private static void signOutGoogle()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();

        if (OnSignOutEvent != null)
        {
            OnSignOutEvent(eRegistrationMethodType.Google, true, eRegistrationResultType.LogInSuccess);
        }
    }

    #endregion

}

public enum eRegistrationMethodType
{
    Google,
}

public enum eRegistrationResultType
{
    AlreadyLoggedIn,
    LogInSuccess,
    LogInFail,
    LogOutSuccess,
    LogOutInFail,
    UnkownService,
}

