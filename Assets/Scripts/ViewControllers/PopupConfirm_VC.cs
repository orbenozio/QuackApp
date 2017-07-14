using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirm_VC : BaseViewController
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private Button _button;

    public Button Button
    {
        get
        {
            return _button;
        }

        set
        {
            _button = value;
        }
    }

    public Text Text
    {
        get
        {
            return text;
        }

        set
        {
            text = value;
        }
    }
}
