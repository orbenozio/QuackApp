using Assets.Scripts.Data;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;
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
    private string _authcode;
    private string _token;
    private string _log;
    private User _user;

    public string Token
    {
        get
        {
            return _token;
        }

        set
        {
            _token = value;
        }
    }

    public string _email { get; private set; }

    public User User
    {
        get
        {
            return _user;
        }
    }

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

    protected override void OnStart()
    {
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
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
         .RequestEmail()
         .RequestServerAuthCode(false)
         .RequestIdToken()
         .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    private void signInGoogle()
    {
        if (((PlayGamesLocalUser)Social.localUser).authenticated)
        {
            if (OnSignInEvent != null)
            {
                OnSignInEvent(eRegistrationMethodType.Google, false, eRegistrationResultType.AlreadyLoggedIn);
            }

            return;
        }

#if UNITY_EDITOR
        _user = new User();
        _user.Username = "Altarf";
        _user.Token = "testToken";
        _user.Id = "testId";
        _user.Email = "or.benozio@gmail.com";
        _user.ChatCount = 0;
        _user.ActiveChats = new Dictionary<string, string>();
        _user.Invites = new Dictionary<string, string>();

        DatabaseService.Instance.SignInUserEvent += handleSignInUserEvent;
        DatabaseService.Instance.SignInUser(User);

        //if (OnSignInEvent != null)
        //{
        //    OnSignInEvent(eRegistrationMethodType.Google, true, eRegistrationResultType.LogInSuccess);
        //    return;
        //}
#endif

        //@TODO: check if user exists in db

        Social.localUser.Authenticate((bool success, string msg) =>
        {
            if (success)
            {
                try
                {
                    _user = new User();
                    _user.Email = ((PlayGamesLocalUser)Social.localUser).Email;
                    _user.Username = ((PlayGamesLocalUser)Social.localUser).userName;
                    _user.Token = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
                    _user.Id = ((PlayGamesLocalUser)Social.localUser).id;
                    _user.ChatCount = 0;
                    _user.ActiveChats = new Dictionary<string, string>();
                    _user.Invites = new Dictionary<string, string>();

                    DatabaseService.Instance.SignInUserEvent += handleSignInUserEvent;
                    DatabaseService.Instance.SignInUser(User);
                }
                catch (Exception e)
                {
                    if (OnSignInEvent != null)
                    {
                        OnSignInEvent(eRegistrationMethodType.Google, true, eRegistrationResultType.LogInFail);
                        return;
                    }
                }
            }
            else
            {

            }
        });
    }

    private void handleSignInUserEvent(bool result)
    {
        if (OnSignInEvent != null)
        {
            OnSignInEvent(eRegistrationMethodType.Google, result, result ? eRegistrationResultType.LogInSuccess : eRegistrationResultType.LogInFail);
            return;
        }
    }

    private UserRequestDelegate HandleSinInUserEvent()
    {
        throw new NotImplementedException();
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

