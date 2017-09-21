using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;

public class SceneCharacterSelection_VC : BaseViewController
{
    public const string CHAT_ROOM_FORAMT = "{0}_{1}_{2}";

    [Header("SceneProperties")]
    [SerializeField]
    private Text _sceneTitle;
    [SerializeField]
    private Text _chatRoomTitle;
    [SerializeField]
    private Image _sceneImage;

    [SerializeField]
    private CharacterSlot_VC _characterSlotPFB;

    [SerializeField]
    private RectTransform _container;


    private Dictionary<string, CharacterSlot_VC> _characterSlots;
    private Dictionary<string, CharacterData> _characters;
    private SceneCharacterSelectionManager _characterSelectionManager;

    protected override void OnAwake()
    {
        base.OnAwake();

    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        _characterSelectionManager = AppManager.Instance.CharacterSelectionManager;

        setSceneVariables();
        setSceneCharacters();
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        _characterSelectionManager.GetCharactersEvent -= handleGetCharactersEvent;

        foreach (var item in _characterSlots)
        {
            item.Value.SlotClickEvent -= handleSlotClickEvent;
            item.Value.DestorySelf();
        }

        _characterSlots.Clear();
    }

    protected override void OnDestroyObject()
    {
        base.OnDestroyObject();
    }

    private void setSceneVariables()
    {
        _characters = new Dictionary<string, CharacterData>();
        _characterSlots = new Dictionary<string, CharacterSlot_VC>();

        var userData = Client.UserData;
        var creatorName = userData.FirstName + "_" + userData.LastName;
        var sceneName = _characterSelectionManager.SelectedSceneData.name;
        var chatCount = userData.ChatCount;
        var chatName = string.Format(CHAT_ROOM_FORAMT, creatorName, sceneName, chatCount + 1);
        var chatData = new ChatData();

        chatData.ChatName = chatName;
        chatData.Creator = creatorName;
        chatData.SceneName = sceneName;
        chatData.UserId = userData.Id;

        _characterSelectionManager.CreateChatRoom(chatData);

        _sceneTitle.text = sceneName;
        _chatRoomTitle.text = chatName;
    }

    private void setSceneCharacters()
    {
        _characterSelectionManager.GetCharactersEvent += handleGetCharactersEvent;
        _characterSelectionManager.GetSceneCharacters();
    }

    private void handleGetCharactersEvent(bool result, Dictionary<string, CharacterData> data)
    {
        _characterSelectionManager.GetCharactersEvent -= handleGetCharactersEvent;

        if (!result)
        {
            return;
        }
        try
        {
            _characters = data;
            _characterSlots.Clear();

            foreach (var character in _characters)
            {
                var instance = Instantiate(_characterSlotPFB);
                instance.gameObject.name = character.Key;
                instance.transform.SetParent(_container, false);
                instance.SlotClickEvent += handleSlotClickEvent;
                instance.Initialize(character.Value);

                _characterSlots.Add(character.Key, instance);
            }
        }
        catch (Exception ex)
        {
            QLogger.LogException(ex);

        }
    }

    private void handleSlotClickEvent(UnityEngine.Object obj)
    {
        var characterSlot = obj as CharacterSlot_VC;

        if (characterSlot == null)
        {
            return;
        }

        characterSlot.SlotClickEvent -= handleSlotClickEvent;

        // Save selected character to current chat
        _characterSelectionManager.SaveUserCharacterToChat(characterSlot.name);
    }
}
