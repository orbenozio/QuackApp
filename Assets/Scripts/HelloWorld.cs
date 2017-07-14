using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class HelloWorld : MonoBehaviour
{
    [SerializeField]
    private GameObject _popupConfirm;
   
    // Use this for initialization
    private void Start()
    {
        RegistrationManager.OnSignInEvent += RegistrationManager_OnSignInEvent;
        RegistrationManager.OnSignOutEvent += RegistrationManager_OnSignOutEvent;
        PopupManager.Instance.OnConfirmButtonClickEvent += Instance_OnConfirmButtonClickEvent;

        RegistrationManager.ActivateServices();
    }
  
    private void OnDestroy()
    {
        RegistrationManager.OnSignInEvent -= RegistrationManager_OnSignInEvent;
        RegistrationManager.OnSignOutEvent -= RegistrationManager_OnSignOutEvent;

        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.OnConfirmButtonClickEvent -= Instance_OnConfirmButtonClickEvent;
        }
    }

    public void SignInGoogle()
    {
        RegistrationManager.SignIn(eRegistrationMethodType.Google);        
    }

    //public void Achivement()
    //{
    //    Social.ReportProgress(GPGSIds.achievement_1, 5, (bool success) =>
    //    {
    //        if (success)
    //        {
    //            QLogger.PrintLog("achivement success");
    //        }
    //        else
    //        {
    //            QLogger.PrintLog("achivement fail");
    //        }
    //    });
    //}

    //public void ShowAchivements()
    //{
    //    Social.ShowAchievementsUI();
    //}


    public void SignOut()
    {
        RegistrationManager.SignOut(eRegistrationMethodType.Google);
    }


    private void RegistrationManager_OnSignInEvent(eRegistrationMethodType reg, bool success, eRegistrationResultType resultType, string message = "")
    {
        if (!success)
        {
            PopupManager.Instance.OnConfirmButtonClickEvent += Instance_OnConfirmButtonClickEvent;
            PopupManager.Instance.ShowConfirmPopup(message);
            return;
        }

        SceneManager.LoadScene("ChatLobby");
    }

    private void Instance_OnConfirmButtonClickEvent()
    {
        PopupManager.Instance.OnConfirmButtonClickEvent -= Instance_OnConfirmButtonClickEvent;

    }

    private void RegistrationManager_OnSignOutEvent(eRegistrationMethodType reg, bool success, eRegistrationResultType resultType, string message = "")
    {
    }
}
