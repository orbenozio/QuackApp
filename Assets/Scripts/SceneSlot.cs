using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void SceneSlotDelegate(SceneSlot obj);

public class SceneSlot : QuackMonoBehaviour
{
    public SceneSlotDelegate ClickEvent;

    [SerializeField]
    private GameObject _slot;

    [SerializeField]
    private Button _clickable;

    [SerializeField]
    private Text _sceneName;

    private SceneData _sceneData;

    public Button Clickable
    {
        get
        {
            return _clickable;
        }
    }

    public Text SceneName
    {
        get
        {
            return _sceneName;
        }
    }
    public SceneData SceneData
    {
        get
        {
            return _sceneData;
        }
    }

    public void Initialize(SceneData sceneData)
    {
        _sceneData = sceneData;
        _sceneName.text = sceneData.name;
    }

    public void OnClick()
    {
        if (ClickEvent != null)
        {
            ClickEvent.Invoke(this);
        }
    }
}
