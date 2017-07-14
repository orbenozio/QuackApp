using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatSlot_VC : BaseViewController
{
    public void OnNewChatClick()
    {
        AppManager.ChatLobbyManager.OnNewChatRoomClick();
    }

    public void OnChatClick(Text name)
    {
        AppManager.ChatLobbyManager.OnChatRoomClick(name.text);
    }
}
