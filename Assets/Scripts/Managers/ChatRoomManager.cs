using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ChatCharacterDataDelegate(List<ChatCharacterData> data = null);

public class ChatRoomManager : QuackMonoBehaviour
{
    public event ChatCharacterDataDelegate ChatCharacterDataEvent;

    private string _currentChatUuid;

    public string ChatUuid
    {
        get
        {
            return _currentChatUuid;
        }
    }

    public void GoToLobby()
    {
        AppManager.Instance.GoToChatLobby();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        //setupChatRoom(_currentChatUuid);
    }

    public void OnChatRoomClick(string chatUuid)
    {
        Debug.Log("RoomClicked - " + chatUuid);

        SetupChatRoom(chatUuid);
    }   

    public void SetupChatRoom(string chatUuid)
    {
        _currentChatUuid = chatUuid;

        AppManager.Instance.UiLoaderManager.SetActive(UILoaderManager.eUIType.ChatRoom, false);

        // Get chat room data;
        DatabaseService.Instance.GetChatCharacterDataEvent += Instance_GetChatCharacterDataEvent;
        DatabaseService.Instance.GetChatRoomData(chatUuid);

    }

    public void ChatRoomReady()
    {
        AppManager.Instance.UiLoaderManager.SetActive(UILoaderManager.eUIType.ChatRoom, true);
    }

    public void SaveRecord(RecordData data)
    {
        DatabaseService.Instance.SaveRecord(data);
    }

    private void Instance_GetChatCharacterDataEvent(bool result, List<ChatCharacterData> data = null)
    {
        DatabaseService.Instance.GetChatCharacterDataEvent -= Instance_GetChatCharacterDataEvent;



        if (ChatCharacterDataEvent != null)
        {
            ChatCharacterDataEvent(data);
        }       
    }

   
}
