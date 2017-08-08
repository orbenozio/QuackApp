using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatLobbyManager : QuackMonoBehaviour
{
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
