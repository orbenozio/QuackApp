using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneCharacterSelectionManager : QuackMonoBehaviour
{
    //public delegate void SceneDelegate(SceneData data);
    
    //public event SceneDelegate SceneSelectedEvent;
    public event CharacterRequestDelegate GetCharactersEvent;

    private SceneData _selectedSceneData;
    private ChatData _currentChatData;

    public SceneData SelectedSceneData
    {
        get
        {
            return _selectedSceneData;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        AppManager.Instance.SceneSelectedEvent += handleSceneSelectedEvent;
    }

    protected override void OnDestroyObject()
    {
        base.OnDestroyObject();

        if (AppManager.Instance != null)
        {
            AppManager.Instance.SceneSelectedEvent -= handleSceneSelectedEvent;
        }
    }

    #region Public Methods

    public void Initialize(SceneData sceneData)
    {
        handleSceneSelectedEvent(sceneData);
    }

    public void GetSceneCharacters()
    {
        DatabaseService.Instance.GetCharactersEvent += handleGetCharactersEvent;
        DatabaseService.Instance.GetSceneCharacters(_selectedSceneData.id);
    }

    public void CreateChatRoom(ChatData chatData)
    {
        _currentChatData = chatData;
        DatabaseService.Instance.CreateChatRoom(chatData);
    }


    public void SaveUserCharacterToChat(string name)
    {
        DatabaseService.Instance.SetChatCharacterEvent += handleSetChatCharacterEvent;

        var chatCharacterData = new ChatCharacterData();
        chatCharacterData.UserId = Client.UserData.Id;
        chatCharacterData.CharacterId = name;
        chatCharacterData.Position = new Vector2(10f, 15f);
        chatCharacterData.IsFullScreen = true;
        DatabaseService.Instance.SetUserChatCharacter(_currentChatData.Uuid, chatCharacterData);
    }    

    #endregion

    #region Private Methods

    private void handleGetCharactersEvent(bool result, Dictionary<string, CharacterData> data)
    {
        DatabaseService.Instance.GetCharactersEvent -= handleGetCharactersEvent;

        var characters = new Dictionary<string, CharacterData>();

        foreach (var kvp in data)
        {
            var characterName = kvp.Key;
            var characterData = kvp.Value;

            if (!characterData.scenes.ContainsKey("scene_" + _selectedSceneData.id))
            {
                continue;
            }

            characters.Add(characterName, characterData);
        }

        if (GetCharactersEvent!=null)
        {
            GetCharactersEvent.Invoke(result, characters);
        }
    }

    private void handleSceneSelectedEvent(SceneData sceneData)
    {
        _selectedSceneData = sceneData;

        //if (SceneSelectedEvent!=null)
        //{
        //    SceneSelectedEvent(_selectedSceneData);
        //}
    }

    private void handleSetChatCharacterEvent(bool result, List<ChatCharacterData> data = null)
    {
        DatabaseService.Instance.SetChatCharacterEvent -= handleSetChatCharacterEvent;

        if (!result)
        {
            return;
        }

        // Go to chat room
        AppManager.Instance.GoToChatRoom(_currentChatData.Uuid);
    }
    #endregion
}
