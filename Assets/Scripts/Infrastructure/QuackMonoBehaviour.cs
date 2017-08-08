using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuackMonoBehaviour : MonoBehaviour
{
    #region Private Methods

    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        OnUpdate();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    private void LateUpdate()
    {
        OnLateUpdate();
    }

    private void OnDestroy()
    {
        OnDestroyObject();
    }

    private void Reset()
    {
        OnReset();
    }

    private void OnEnable()
    {
        OnEnableObject();
    }

    private void OnDisable()
    {
        OnDisableObject();
    }

    #endregion

    #region Protected Virtual Methods

    protected virtual void OnAwake()
    {
    }

    protected virtual void OnStart()
    {
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnFixedUpdate()
    {
    }

    protected virtual void OnLateUpdate()
    {
    }

    protected virtual void OnDestroyObject()
    {
    }

    protected virtual void OnReset()
    {
    }

    protected virtual void OnEnableObject()
    {
    }

    protected virtual void OnDisableObject()
    {
    }

    public virtual void DestorySelf()
    {
        Destroy(this.gameObject);
    }


    #endregion

}
