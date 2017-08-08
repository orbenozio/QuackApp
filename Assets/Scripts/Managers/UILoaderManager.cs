using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILoaderManager : QuackMonoBehaviour
{
    #region Inspector

    [SerializeField]
    private Transform _uiCanvas;

    [SerializeField]
    List<UIPrefabComponent> _uiPrefabs;

    #endregion

    #region Private Fields

    Dictionary<eUIType, IViewController> _cachedUI;

    #endregion

    #region Quack Mono Behaviour

    protected override void OnAwake()
    {
        _cachedUI = new Dictionary<eUIType, IViewController>();

        //if (AppManager.Instance != null)
        //{
        //    AppManager.Instance.SceneSelectedEvent += HandleSceneSelectedEvent;
        //}
    }

    protected override void OnDestroyObject()
    {
        base.OnDestroyObject();

        //if (AppManager.Instance != null)
        //{
        //    AppManager.Instance.SceneSelectedEvent -= HandleSceneSelectedEvent;
        //}
    }

    #endregion

    public void SetActive(eUIType uiType, bool state)
    {
        foreach (var uiItem in _cachedUI)
        {
            uiItem.Value.SetActive(false);
            //Destroy(((BaseViewController)uiItem.Value).gameObject);
        }

        if (!_cachedUI.ContainsKey(uiType))
        {
            var prefabToInstantiate = _uiPrefabs.FirstOrDefault(i => i.UiType.Equals(uiType));

            if (prefabToInstantiate == null)
            {
                throw new MissingReferenceException(String.Format("Couldn't find ui prefabe of type [{0}]", uiType));
            }

            var instantiatedGameObject = Instantiate(prefabToInstantiate.UiPrefab);
            _cachedUI.Add(uiType, ((IViewController)instantiatedGameObject.GetComponent<BaseViewController>()));
            _cachedUI[uiType].Initialize(_uiCanvas);
        }
        else
        {
            _cachedUI[uiType].SetActive(state);
        }
    }


    private void HandleSceneSelectedEvent(SceneData data)
    {
        //SetActive(UILoaderManager.eUIType.SceneSelection, false);
        SetActive(UILoaderManager.eUIType.CharacterSelection, true);
    }



    [Serializable]
    public class UIPrefabComponent
    {
        [SerializeField]
        private GameObject _uiPrefab;

        [SerializeField]
        private eUIType _uiType;

        public GameObject UiPrefab
        {
            get
            {
                return _uiPrefab;
            }

            set
            {
                _uiPrefab = value;
            }
        }

        public eUIType UiType
        {
            get
            {
                return _uiType;
            }

            set
            {
                _uiType = value;
            }
        }
    }

    public enum eUIType
    {
        Registration,
        ChatLobby,
        Popup,
        SceneSelection,
        CharacterSelection,
        ChatRoom,
    }
}
