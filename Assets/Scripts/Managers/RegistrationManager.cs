using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using Facebook.Unity;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RegistrationDelegte(eRegistrationMethodType reg, bool success, eRegistrationResultType resultType, string message = "");

public class RegistrationManager : QuackMonoBehaviour
{
    #region Consts

    private readonly List<string> FACEBOOK_READ_PERMISSIONS = new List<string>() { "email", "user_friends" };

    #endregion

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
    protected override void OnAwake()
    {
        base.OnAwake();

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }
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
            case eRegistrationMethodType.Facebook:
                {
                    signInFacebook();
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

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
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

    private void signInFacebook()
    {
        FB.LogInWithReadPermissions(FACEBOOK_READ_PERMISSIONS, authCallback);
    }

    private void authCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            FB.API("/me?fields=name,email", HttpMethod.GET, facebookLoginCallback);
          
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void facebookLoginCallback(IGraphResult result)
    {
        if (result.Error != null)
        {
            QLogger.LogError("Error Response:\n" + result.Error);
        }
        else if (!FB.IsLoggedIn)
        {
            QLogger.LogError("Login cancelled by Player");
        }
        else
        {
            IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
            var fbname = dict["name"].ToString();

            _user = new User();
            _user.Username = dict["name"].ToString();
            _user.Token = AccessToken.CurrentAccessToken.TokenString;
            _user.Id = AccessToken.CurrentAccessToken.UserId;
            _user.Email = dict["email"].ToString();
            _user.ChatCount = 0;
            _user.ActiveChats = new Dictionary<string, string>();
            _user.Invites = new Dictionary<string, string>();

            DatabaseService.Instance.SignInUserEvent += handleSignInUserEvent;
            DatabaseService.Instance.SignInUser(User);
        }
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
    Facebook,
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

