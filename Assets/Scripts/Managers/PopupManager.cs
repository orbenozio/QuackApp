using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : QuackMonoBehaviour
{
    public event Action OnConfirmButtonClickEvent;

    [SerializeField]
    private PopupConfirm_VC _popupConfirm;

    public void ShowConfirmPopup(string message)
    {
        _popupConfirm.Button.onClick.AddListener(() => closePopup());
        _popupConfirm.Text.text = message;

        _popupConfirm.gameObject.SetActive(true);
    }

    private void closePopup()
    {
        QLogger.Log("Close popup Clicked!");
        _popupConfirm.gameObject.SetActive(false);

    }
}
