using Assets.Scripts.Data;
using Assets.Scripts.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterSlot_VC : QuackMonoBehaviour, ISlot
{
    public event SlotDelegate SlotClickEvent;

    protected readonly int PHASE_IDLE_HASH = Animator.StringToHash("idle");
    protected readonly int PHASE_ONE_HASH = Animator.StringToHash("phase_1");
    protected readonly int PHASE_TWO_HASH = Animator.StringToHash("phase_2");

    [SerializeField]
    private GameObject _slot;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Text _characterName;

    private bool _wasTappedOnce;
    private CharacterData _characterData;

    public Animator Animator
    {
        get
        {
            return _animator;
        }
    }

    public CharacterData CharacterData
    {
        get
        {
            return _characterData;
        }
    }

    public void Initialize(CharacterData data)
    {
        _characterData = data;
        _characterName.text = data.name;

        AnimatorOverrideController slotOverrideController = (AnimatorOverrideController)Resources.Load("Animations\\Controllers\\" + _characterData.animationController);
        slotOverrideController.runtimeAnimatorController = _animator.runtimeAnimatorController;

        // Put this line at the end because when you assign a controller on an Animator, unity rebind all the animated properties 
        _animator.runtimeAnimatorController = slotOverrideController;
    }

   
    public void OnCharacterTap()
    {
        if (!_wasTappedOnce)
        {
            _animator.SetTrigger(PHASE_ONE_HASH);
            _wasTappedOnce = true;
            return;
        }

        if (SlotClickEvent != null)
        {
            SlotClickEvent(this);
        }
    }
}