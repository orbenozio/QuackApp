using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatLobbyManager : QuackMonoBehaviour
{
    [SerializeField]
    private ChatSlot_VC _chatLobbyUI;

    public void OnNewChatRoomClick()
    {
        Debug.Log("Start new chat");
        AppManager.Instance.NewChatRequest();
    }

    public void OnChatRoomClick(string name)
    {
        Debug.Log("RoomClicked - " + name);
    }

}
