using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChatLobby_VC : BaseViewController
{
    [SerializeField]
    private ChatSlot_VC _chatSlotPFB;

    [SerializeField]
    private RectTransform _activeChatsContainer;

    [SerializeField]
    private RectTransform _inviteChatsContainer;

    private Dictionary<string, ChatSlot_VC> _userChats;
    private Dictionary<string, ChatSlot_VC> _userInvites;

    protected override void OnStart()
    {
        base.OnStart();

        _userChats = new Dictionary<string, ChatSlot_VC>();
        _userInvites = new Dictionary<string, ChatSlot_VC>();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        setUserActiveChats();
        setUserInvites();
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        DatabaseService.Instance.GetActiveChatEvent -= handleGetActiveChatEvent;
        DatabaseService.Instance.GetInviteChatEvent -= handleGetInviteChatEvent;

        foreach (var item in _userChats)
        {
            item.Value.DestorySelf();
        }

        foreach (var item in _userInvites)
        {
            item.Value.DestorySelf();
        }

        _userChats.Clear();
        _userInvites.Clear();
    }

    public void OnNewChatClick()
    {      
        AppManager.ChatLobbyManager.OnNewChatRoomClick();
    }

    public void OnChatClick(Text name)
    {
      
        AppManager.ChatLobbyManager.OnChatRoomClick(name.text);
    }

    private void setUserActiveChats()
    {
        DatabaseService.Instance.GetActiveChatEvent += handleGetActiveChatEvent;
        DatabaseService.Instance.GetUserActiveChats(Client.UserData.Id);
    }

    private void setUserInvites()
    {
        DatabaseService.Instance.GetInviteChatEvent += handleGetInviteChatEvent;
        DatabaseService.Instance.GetUserInviteChats(Client.UserData.Id);
    }
    
    private void handleGetActiveChatEvent(bool result, Dictionary<string, string> data)
    {
        DatabaseService.Instance.GetActiveChatEvent -= handleGetActiveChatEvent;

        if (!result)
        {
            return;
        }

        foreach (var kvp in data)
        {
            var chatId = kvp.Key;
            var chatName = kvp.Value;


            var instance = Instantiate(_chatSlotPFB);
            instance.gameObject.name = chatName;
            instance.transform.SetParent(_activeChatsContainer, false);
            //instance.ClickEvent += sceneSelected;
            instance.Initialize(chatId, chatName);

            _userChats.Add(chatId, instance);
            //_availableScenes.Add(sceneSlotInstance);
        }

    }

    private void handleGetInviteChatEvent(bool result, Dictionary<string, string> data)
    {
        DatabaseService.Instance.GetInviteChatEvent -= handleGetInviteChatEvent;

        if (!result)
        {
            return;
        }

        foreach (var kvp in data)
        {
            var chatId = kvp.Key;
            var chatName = kvp.Value;


            var instance = Instantiate(_chatSlotPFB);
            instance.gameObject.name = chatName;
            instance.transform.SetParent(_inviteChatsContainer, false);

            _userInvites.Add(chatId, instance);

            //instance.ClickEvent += sceneSelected;
            //instance.Initialize(scene.Value);

            //_availableScenes.Add(sceneSlotInstance);
        }

    }
}