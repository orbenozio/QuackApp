using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : BaseSingleton<AppManager>
{    
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

    #region Private Fields

    private ChatLobbyManager _chatLobbyManager;
    private PopupManager _popupManager;
    private RegistrationManager _registrationManager;
    private SceneSelectionManager _sceneSelectionManager;

    #endregion


    protected AppManager()
    {
    }

    #region Quack Mono Behaviour

    protected override void OnAwake()
    {
        initManagers();
    }
  
    protected override void OnStart()
    {
        _registrationManager.OnSignInEvent += registrationManager_OnSignInEvent;
        _uiLoaderManager.SetActive(UILoaderManager.eUIType.Registration, true);
    }

    protected override void OnDestroyObject()
    {
        _registrationManager.OnSignInEvent -= registrationManager_OnSignInEvent;

    }

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

    #endregion

    #region Public Methods

    public void NewChatRequest()
    {
        _uiLoaderManager.SetActive(UILoaderManager.eUIType.ChatLobby, false);
        _uiLoaderManager.SetActive(UILoaderManager.eUIType.SceneSelection, true);
    }

    #endregion

    #region Private Methods

    private void initManagers()
    {
        _registrationManager = instantiateManager<RegistrationManager>(_registrationManagerPFB);
        _chatLobbyManager = instantiateManager<ChatLobbyManager>(_chatLobbyManagerPFB);
        _popupManager = instantiateManager<PopupManager>(_popupManagerPFB);
        _sceneSelectionManager = instantiateManager<SceneSelectionManager>(_sceneSelectionManagerPFB);
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
        _uiLoaderManager.SetActive(UILoaderManager.eUIType.Registration, false);
    }


    #endregion
}
