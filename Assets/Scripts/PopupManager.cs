using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : BaseSingleton<PopupManager>
{
    public event Action OnConfirmButtonClickEvent;

    [SerializeField]
    private GameObject _popupConfirm;

    [SerializeField]
    private Text _message;

    [SerializeField]
    private Button _confirmButton;

    protected PopupManager() { }

    public void ShowConfirmPopup(string message)
    {
        _confirmButton.onClick.AddListener(() => closePopup());
        _message.text = message;

        _popupConfirm.SetActive(true);
    }

    private void closePopup()
    {
        QLogger.PrintLog("Close popup Clicked!");
        _popupConfirm.SetActive(false);

    }
}
