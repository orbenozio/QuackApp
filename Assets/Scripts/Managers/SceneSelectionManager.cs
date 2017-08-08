using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSelectionManager : QuackMonoBehaviour
{
    public void OnSceneClick(SceneData data)
    {
        AppManager.Instance.SceneSelectionRequest(data);
    }
}
