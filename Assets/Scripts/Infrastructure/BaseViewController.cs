using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseViewController : QuackMonoBehaviour, IViewController
{
    private Transform _uiParent;

    public AppManager AppManager
    {
        get
        {
            return AppManager.Instance;
        }
    }

    public void Initialize(Transform parent)
    {
        _uiParent = parent;
        this.transform.SetParent(_uiParent, false);

        OnInitialize();
    }

    public void SetActive(bool state)
    {
        bool changeState = (state && !gameObject.activeInHierarchy) || (!state && gameObject.activeInHierarchy);

        if (changeState)
        {
            this.gameObject.SetActive(state);
        }
    }


    protected virtual void OnInitialize()
    {
    }

}
