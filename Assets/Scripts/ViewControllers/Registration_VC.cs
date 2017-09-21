using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Registration_VC : BaseViewController
{
    #region Inspector

    [SerializeField]
    private Button _signInButton;

    #endregion

    #region Props

    public Button SignInButton
    {
        get
        {
            return _signInButton;
        }
    }

    #endregion

    #region Public Methods

    public void OnSignInGoogleClick()
    {
        AppManager.RegistrationManager.SignIn(eRegistrationMethodType.Google);
    }

    public void OnSignInFaecbookClick()
    {
        AppManager.RegistrationManager.SignIn(eRegistrationMethodType.Facebook);
    }

    #endregion
}
