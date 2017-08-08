using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.Data;

public class SceneSelection_VC : BaseViewController
{
    [SerializeField]
    private SceneSlot _sceneSlotPFB;

    [SerializeField]
    private RectTransform _scenesContainer;

    private Dictionary<string, SceneData> _scenes;
    private Dictionary<string, SceneSlot> _availableScenes;

    protected override void OnAwake()
    {
        base.OnAwake();

        _availableScenes = new Dictionary<string, SceneSlot>();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();
        
        // Get available scenes from database
        setAvailableScenes();
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        foreach (var item in _availableScenes)
        {
            item.Value.ClickEvent -= sceneSelected;
            item.Value.DestorySelf();
        }

        _availableScenes.Clear();

        if (DatabaseService.Instance != null)
        {
            DatabaseService.Instance.GetScenesEvent -= HandleGetScenesEvent;
        }
    }

    protected override void OnDestroyObject()
    {
        base.OnDestroyObject();

        foreach (var item in _availableScenes)
        {
            item.Value.ClickEvent -= sceneSelected;
            item.Value.DestorySelf();
        }

        _availableScenes.Clear();

        if (DatabaseService.Instance != null)
        {
            DatabaseService.Instance.GetScenesEvent -= HandleGetScenesEvent;
        }
    }

    private void setAvailableScenes()
    {
        DatabaseService.Instance.GetScenesEvent += HandleGetScenesEvent;
        DatabaseService.Instance.GetAvailableScenes();
    }

    private void HandleGetScenesEvent(bool result, Dictionary<string, SceneData> data)
    {
        DatabaseService.Instance.GetScenesEvent -= HandleGetScenesEvent;

        if (!result)
        {
            return;
        }

        _scenes = data;

        foreach (var scene in _scenes)
        {
            SceneSlot sceneSlotInstance = null;

            if (_availableScenes.ContainsKey(scene.Key))
            {
                sceneSlotInstance = _availableScenes[scene.Key];
            }
            else
            { 
                sceneSlotInstance = Instantiate(_sceneSlotPFB);
                sceneSlotInstance.gameObject.name = scene.Key;
                sceneSlotInstance.transform.SetParent(_scenesContainer, false);
                _availableScenes.Add(scene.Key, sceneSlotInstance);
            }

            sceneSlotInstance.ClickEvent += sceneSelected;
            sceneSlotInstance.Initialize(scene.Value);
        }
    }

    private void sceneSelected(SceneSlot scene)
    {
        scene.ClickEvent -= sceneSelected;

        Debug.Log("Scene " + scene.SceneName.text + " was selected");
        AppManager.SceneSelectionManager.OnSceneClick(scene.SceneData);
    }
}
