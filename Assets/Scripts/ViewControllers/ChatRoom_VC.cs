using Assets.Scripts.Data;
using Assets.Scripts.ViewControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoom_VC : BaseViewController
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private ChatCharacter_VC _chatCharacterPFB;
    [SerializeField] private RectTransform _container;

    private Dictionary<string, ChatCharacter_VC> _characters;

    protected override void OnAwake()
    {
        base.OnAwake();
        _characters = new Dictionary<string, ChatCharacter_VC>();

        AppManager.Instance.ChatRoomManager.ChatCharacterDataEvent += ChatRoomManager_ChatCharacterDataEvent;
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnDestroyObject()
    {
        base.OnDestroyObject();

        foreach (var item in _characters)
        {
            item.Value.DestorySelf();
        }

        _characters.Clear();
    }

    public void OnBack_Click()
    {
        foreach (var item in _characters)
        {
            item.Value.DestorySelf();
        }

        _characters.Clear();

        // Go to chat lobby
        AppManager.ChatRoomManager.GoToLobby();
    }

    public void OnInvite_Click()
    {

    }

    private void ChatRoomManager_ChatCharacterDataEvent(List<ChatCharacterData> data = null)
    {
        // set background
        _backgroundImage.color = new Color(Random.value, Random.value, Random.value, 1.0f);

        // set user position
        foreach (var characterData in data)
        {
            var instance = Instantiate(_chatCharacterPFB);
            instance.gameObject.name = characterData.UserId;
            instance.transform.position = new Vector3(characterData.Position.x, characterData.Position.y, 0f);
            instance.transform.SetParent(_container, false);
            //instance.SlotClickEvent += handleSlotClickEvent;
            instance.Initialize(characterData);

            _characters.Add(characterData.UserId, instance);
        }

        // set user character

        // set other members characters + positions

        // set press to talk button  

        // set timeline

        AppManager.Instance.ChatRoomManager.ChatRoomReady();

    }
}
