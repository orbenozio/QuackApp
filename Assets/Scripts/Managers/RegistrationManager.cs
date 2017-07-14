using GooglePlayGames;
using System;
using UnityEngine;

public delegate void RegistrationDelegte(eRegistrationMethodType reg, bool success, eRegistrationResultType resultType, string message = "");

public class RegistrationManager : QuackMonoBehaviour
{
    #region Events

    public event RegistrationDelegte OnSignInEvent;
    public event RegistrationDelegte OnSignOutEvent;

    #endregion

    #region Inspector

    [SerializeField]
    private GameObject _registrationUI;

    #endregion

    #region Private Fields

    //private GameObject _registrationGameObject;

    #endregion

    #region Props

    //public GameObject RegistrationGameObject
    //{
    //    get
    //    {
    //        return _registrationGameObject;
    //    }

    //    set
    //    {
    //        _registrationGameObject = value;
    //    }
    //}

    #endregion

    #region Quack Mono Behaviour

    protected override void OnAwake()
    {
        //_registrationGameObject = Instantiate(_registrationUI);
        activateServices();
    }

    #endregion

    #region Public Methods   

    public void SignIn(eRegistrationMethodType registrationMethodType)
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

    public void SignOut(eRegistrationMethodType registrationMethodType)
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

    private void activateServices()
    {
        PlayGamesPlatform.Activate();
    }

    private void signInGoogle()
    {
#if UNITY_EDITOR
        if (OnSignInEvent != null)
        {
            OnSignInEvent(eRegistrationMethodType.Google, true, eRegistrationResultType.LogInSuccess);
        }

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

    private void signOutGoogle()
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

