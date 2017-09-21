using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using Facebook.Unity;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void RegistrationDelegte(eRegistrationMethodType reg, bool success, eRegistrationResultType resultType, string message = "");

public class RegistrationManager : QuackMonoBehaviour
{
    #region Consts

    private readonly List<string> FAECBOOK_READ_PERMISSIONS = new List<string>() { "public_profile", "email", "user_friends" };

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

    public bool IsLoggedIn
    {
        get
        {
            return FB.IsLoggedIn;
        }
    }
    #endregion

    #region Quack Mono Behaviour

    protected override void OnAwake()
    {
        base.OnAwake();

        // Initialize Facebook
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(initCallback, onHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            initCallback();
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

    #region Faecbook

    private void initCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();

            if (PlayerPrefsHelper.GetString(PlayerPrefsConsts.PP_LOGIN_METHOD) == PlayerPrefsConsts.PP_FACEBOOK_LOGIN)
            {
                signInFacebook();
            }

            //if (FB.IsLoggedIn)
            //{
            //    // If exist in DB get the user data
            //    var aToken = AccessToken.CurrentAccessToken;

            //    DatabaseService.Instance.GetUserDataEvent += handleGetUserDataEvent;
            //    DatabaseService.Instance.GetUserDataById(aToken.UserId);
            //}
            //else
            //{
            //    if (OnSignInEvent != null)
            //    {

            //    }
            //}

        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void onHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void signInFacebook()
    {
     
        FB.LogInWithReadPermissions(FAECBOOK_READ_PERMISSIONS, authCallback);
    }


    private void authCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            PlayerPrefsHelper.SetString(PlayerPrefsConsts.PP_LOGIN_METHOD, PlayerPrefsConsts.PP_FACEBOOK_LOGIN);

            // If exist in DB get the user data
            var aToken = AccessToken.CurrentAccessToken;
            
            DatabaseService.Instance.GetUserDataEvent += handleGetUserDataEvent;
            DatabaseService.Instance.GetUserDataById(aToken.UserId);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void handleGetUserDataEvent(bool result, User userData)
    {
        DatabaseService.Instance.GetUserDataEvent -= handleGetUserDataEvent;

        if (userData == null)
        {
            // Create new user
            FB.API("/me?fields=first_name,last_name,email", HttpMethod.GET, createNewUserData);
            return;
        }

        Client.UserData = userData;

        if (OnSignInEvent != null)
        {
            OnSignInEvent(eRegistrationMethodType.Facebook, true, eRegistrationResultType.LogInSuccess);
        }
    }

    private void createNewUserData(IGraphResult result)
    {
        Client.UserData = new User();

        var userData = JSONSerialization<User>.CreateFromJSON(result.RawResult);
        Client.UserData = userData;
        Client.UserData.Token = AccessToken.CurrentAccessToken.TokenString;
        Client.UserData.ChatCount = 0;
        Client.UserData.ActiveChats = new Dictionary<string, string>();
        Client.UserData.Invites = new Dictionary<string, string>();

        DatabaseService.Instance.CreateUserDataEvent += handleCreateUserDataEvent;
        DatabaseService.Instance.CreateNewUser(Client.UserData);
    }

    private void handleCreateUserDataEvent(bool result, User userData = null)
    {
        DatabaseService.Instance.CreateUserDataEvent -= handleCreateUserDataEvent;

        if (OnSignInEvent != null)
        {
            OnSignInEvent(eRegistrationMethodType.Facebook, true, eRegistrationResultType.LogInSuccess);
        }
    }

    #endregion
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

        //DatabaseService.Instance.SignInUserEvent += handleSignInUserEvent;
        //DatabaseService.Instance.SignInUser(User);

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

                    //DatabaseService.Instance.SignInUserEvent += handleSignInUserEvent;
                    //DatabaseService.Instance.SignInUser(User);
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

    //private UserRequestDelegate HandleSinInUserEvent()
    //{
    //    throw new NotImplementedException();
    //}

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

