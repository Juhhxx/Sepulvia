using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Events;

public class PullingManager : RandomBehaviour
{
    // GameObjects
    [SerializeField] private PullingUIManager _pullingUIManager;

    public void TogglePullUI(bool onOff) => _pullingUIManager.TogglePullUI(onOff);
    public void ToggleBarButtons(bool onOff) => _pullingUIManager.ToggleBarButtons(onOff);

    // Pulling Variables
    [SerializeField] private float _pullSpeed = 0.2f;

    [SerializeField, ReadOnly] private List<BarSection> _barSectionList;
    public List<BarSection> BarSections => _barSectionList;
    private int _sectionsNumber;

    // Hearth Variables
    private int _currentHeartIndex;

    // Bar Selection Logic
    [field: SerializeField, ReadOnly] public int SelectedIndex { get; private set; }
    public void SetSelectedIndex(int index)
    {
        SelectedIndex = index;
        OnSelectBar?.Invoke(index);
    }

    private void SetUpBarButtons()
    {
        for (int i = 0; i < _barSectionList.Count; i++)
        {
            int index = i;

            _barSectionList[i].Button.onClick.AddListener(() => SetSelectedIndex(index));
        }
    }

    // Events
    public event Action<int> OnSelectBar;
    public event Action<bool> OnHeartEnd;
    public void ResetEvents()
    {
        OnSelectBar = null;
        OnHeartEnd = null;
    }

    public UnityEvent OnSoulMove;

    // Set Up
    public void SetUp(Party enemyParty)
    {
        if (_random == null) TryInitializeRandom();

        //  Calculate number of sections based on enemy party
        _sectionsNumber = (enemyParty.PartySize * 5) + 5;

        _pullingUIManager.SpawnHeart();
        _pullingUIManager.SpawnBarSections(_sectionsNumber);

        _barSectionList = _pullingUIManager.GetBarSections();

        SetUpBarButtons();
        _pullingUIManager.ToggleBarButtons(false);

        SetHeartInMiddle();
    }

    private void SetHeartInMiddle()
    {
        if (_sectionsNumber % 2 == 0)
        {
            int rnd = _random.Next(0, 1);

            _currentHeartIndex = (_sectionsNumber / 2) + rnd;

            _pullingUIManager.MoveHeart(_currentHeartIndex, doAnim: false);
        }
        else
        {
            _currentHeartIndex = (_sectionsNumber / 2) + 1;
            _pullingUIManager.MoveHeart(_currentHeartIndex, doAnim: false);
        }
    }

    // Moving Logic
    public bool IsMoving { get; private set; }

    public void MoveHeart(int pushForce)
    {
        StartCoroutine(MoveHeartCR(Mathf.Abs(pushForce), pushForce > 0));
    }
    private IEnumerator MoveHeartCR(int pushForce, bool positive)
    {
        IsMoving = true;

        for (int i = 0; i < pushForce; i++)
        {
            BarSection section = _barSectionList[_currentHeartIndex];

            if (section.HasModifier)
            {
                if (DoBarModifier(section)) 
                {
                    IsMoving = false;
                    yield break;
                }
            }

            var tmp = _currentHeartIndex;

            if (positive) tmp++;
            else tmp--;

            Debug.Log($"GOING TO {tmp}");

            if (tmp < 0)
            {
                _pullingUIManager.MoveHeart(tmp);
                OnHeartEnd?.Invoke(true);
                IsMoving = false;
                yield break;
            }
            else if (tmp >= _barSectionList.Count)
            {
                _pullingUIManager.MoveHeart(tmp);
                OnHeartEnd?.Invoke(false);
                IsMoving = false;
                yield break;
            }
            else
            {
                section.SetHasHeart(false);

                if (_barSectionList[tmp].HasModifier)
                {
                    if (DoBarModifier(_barSectionList[tmp])) 
                    {
                        IsMoving = false;
                        yield break;
                    }
                }
                
                _currentHeartIndex = tmp;

                _pullingUIManager.MoveHeart(_currentHeartIndex, () => OnSoulMove?.Invoke());
            }

            yield return new WaitForSeconds(_pullSpeed);
        }

        IsMoving = false;
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        IsMoving = false;
    }

    // Modifier Logic
    public void CheckBarModifiers()
    {
        foreach (BarSection section in _barSectionList)
        {
            if (section == null) continue;
            if (!section.HasModifier) continue;

            section.BarModifier.TurnPassed();

            // if (section.BarModifier.CheckIfAlmostDone()) DialogueManager.Instance.AddDialogue($"{section.BarModifier.Type.ToTitle()} was destroyed.");

            if (section.BarModifier.CheckIfDone())
            {
                section.RemoveBarModifier();
                // DialogueManager.Instance.StartDialogues($"{section.BarModifier.Type.ToTitle()} was destroyed.");
            }
        }
    }
    public bool DoBarModifier(BarSection section, BarModifierTrigger trigger = BarModifierTrigger.OnStartTurn)
    {
        // if (section.BarModifier.Trigger != trigger) return false;

        bool stopMovement = false;

        switch (section.BarModifier.Type)
        {
            case BarModifierType.Barrier:
                
                stopMovement = true;
                DialogueManager.Instance.AddDialogue($"A Barrier Stopped the Movement.");
                break;
        }

        if (section.BarModifier.DestroyOnUse) section.RemoveBarModifier();

        return stopMovement;
    }

}
