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
        DatabaseService.Instance.GetActiveChatEvent -= HandleGetActiveChatEvent;
        DatabaseService.Instance.GetInviteChatEvent -= HandleGetInviteChatEvent;

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
        var userId = AppManager.RegistrationManager.User.Id;
        DatabaseService.Instance.GetActiveChatEvent += HandleGetActiveChatEvent;
        DatabaseService.Instance.GetUserActiveChats(userId);
    }

    private void setUserInvites()
    {
        var userId = AppManager.Instance.RegistrationManager.User.Id;
        DatabaseService.Instance.GetInviteChatEvent += HandleGetInviteChatEvent;
        DatabaseService.Instance.GetUserInviteChats(userId);
    }
    
    private void HandleGetActiveChatEvent(bool result, Dictionary<string, string> data)
    {
        DatabaseService.Instance.GetActiveChatEvent -= HandleGetActiveChatEvent;

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

    private void HandleGetInviteChatEvent(bool result, Dictionary<string, string> data)
    {
        DatabaseService.Instance.GetInviteChatEvent -= HandleGetInviteChatEvent;

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