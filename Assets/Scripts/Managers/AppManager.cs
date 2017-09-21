using Assets.Scripts.Data;
using System;
using UnityEngine;


public class AppManager : BaseSingleton<AppManager>
{
    public delegate void SceneDelegate(SceneData data);

    #region Events

    public event SceneDelegate SceneSelectedEvent;

    #endregion

    [Header("Managers")]
    [SerializeField]
    private Transform _managersContainer;

    [SerializeField]
    private UILoaderManager _uiLoaderManager;

    [SerializeField]
    private ChatLobbyManager _chatLobbyManagerPFB;    

    [SerializeField]
    private PopupManager _popupManagerPFB;

    [SerializeField]
    private RegistrationManager _registrationManagerPFB;

    [SerializeField]
    private SceneSelectionManager _sceneSelectionManagerPFB;    

    [SerializeField]
    private SceneCharacterSelectionManager _characterSelectionManagerPFB;

    [SerializeField]
    private ChatRoomManager _chatRoomManagerPFB;

    #region Private Fields

    private ChatLobbyManager _chatLobbyManager;
    private PopupManager _popupManager;
    private RegistrationManager _registrationManager;
    private SceneSelectionManager _sceneSelectionManager;
    private SceneCharacterSelectionManager _characterSelectionManager;
    private ChatRoomManager _chatRoomManager;

    #endregion

    #region Props

    public ChatLobbyManager ChatLobbyManager
    {
        get
        {
            return _chatLobbyManager;
        }
    }

    public PopupManager PopupManager
    {
        get
        {
            return _popupManager;
        }
    }

    public RegistrationManager RegistrationManager
    {
        get
        {
            return _registrationManager;
        }
    }

    public SceneCharacterSelectionManager CharacterSelectionManager
    {
        get
        {
            return _characterSelectionManager;
        }

        set
        {
            _characterSelectionManager = value;
        }
    }

    public SceneSelectionManager SceneSelectionManager
    {
        get
        {
            return _sceneSelectionManager;
        }

        set
        {
            _sceneSelectionManager = value;
        }
    }

    public ChatRoomManager ChatRoomManager
    {
        get
        {
            return _chatRoomManager;
        }

        set
        {
            _chatRoomManager = value;
        }
    }

    public UILoaderManager UiLoaderManager
    {
        get
        {
            return _uiLoaderManager;
        }
    }

    public User UserData
    {
        get
        {
            return this.RegistrationManager.User;
        }
    }

    public string ChatUuid
    {
        get
        {
            return this.ChatRoomManager.ChatUuid;
        }
    }
    #endregion

    protected AppManager()
    {
    }

    #region Quack Mono Behaviour

    protected override void OnAwake()
    {
        initServices();
        initManagers();        
    }

    protected override void OnStart()
    {
        _registrationManager.OnSignInEvent += registrationManager_OnSignInEvent;

        if (PlayerPrefsHelper.GetString(PlayerPrefsConsts.PP_LOGIN_METHOD) != PlayerPrefsConsts.PP_FACEBOOK_LOGIN)
        {
            _uiLoaderManager.SetActive(UILoaderManager.eUIType.Registration, true);
        }
    }

    protected override void OnDestroyObject()
    {
        _registrationManager.OnSignInEvent -= registrationManager_OnSignInEvent;

    }

    #endregion

    #region Public Methods

    public void NewChatRequest()
    {
        _uiLoaderManager.SetActive(UILoaderManager.eUIType.SceneSelection, true);
    }

    public void SceneSelectionRequest(SceneData data)
    {
        // Setup
        _characterSelectionManager.Initialize(data);

        //if (SceneSelectedEvent!=null)
        //{
        //    SceneSelectedEvent.Invoke(data);
        //}     
        _uiLoaderManager.SetActive(UILoaderManager.eUIType.CharacterSelection, true);

    }

    public void GoToChatLobby()
    {
        _uiLoaderManager.SetActive(UILoaderManager.eUIType.ChatLobby, true);
    }

    public void GoToChatRoom(string chatUuid)
    {
        _chatRoomManager.SetupChatRoom(chatUuid);
    }

    #endregion

    #region Private Methods

    private void initServices()
    {
        DatabaseService.Instance.InitializeFirebase();
    }


    private void initManagers()
    {        
        _registrationManager = instantiateManager<RegistrationManager>(_registrationManagerPFB);
        _chatLobbyManager = instantiateManager<ChatLobbyManager>(_chatLobbyManagerPFB);
        _popupManager = instantiateManager<PopupManager>(_popupManagerPFB);
        _sceneSelectionManager = instantiateManager<SceneSelectionManager>(_sceneSelectionManagerPFB);
        _characterSelectionManager = instantiateManager<SceneCharacterSelectionManager>(_characterSelectionManagerPFB);
        _chatRoomManager = instantiateManager<ChatRoomManager>(_chatRoomManagerPFB);
    }

    private T instantiateManager<T>(T manager) where T : QuackMonoBehaviour
    {
        var go = Instantiate(manager);
        go.transform.SetParent(_managersContainer);
        go.name = manager.name;

        return go;
    }

    #endregion

    #region Event Methods

    private void registrationManager_OnSignInEvent(eRegistrationMethodType reg, bool success, eRegistrationResultType resultType, string message = "")
    {
        if (!success)
        {
            AppManager.Instance.PopupManager.ShowConfirmPopup(message);
            return;
        }

        _uiLoaderManager.SetActive(UILoaderManager.eUIType.ChatLobby, true);
    }


    #endregion
}
