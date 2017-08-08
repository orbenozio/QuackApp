using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatSlot_VC : BaseViewController
{
    [SerializeField]
    private Text _chatName;

    private string _uuid;

    public void Initialize(string uuid, string name)
    {
        _uuid = uuid;
        _chatName.text = name;
    }
    public void OnChat_Click()
    {
        AppManager.ChatRoomManager.OnChatRoomClick(_uuid);
    }
}
