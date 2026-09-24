using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Events;
using Unity.VisualScripting;

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

    public void BreakBarSection(BarSection section, bool burn = false)
    {
        if (section.HasModifier) section.RemoveBarModifier();

        if (section.ConnectLeft != null) section.ConnectLeft.ConnectRight = section.ConnectRight;
        if (section.ConnectRight != null) section.ConnectRight.ConnectLeft = section.ConnectLeft;

        if (!burn) section.DestroySection();
        else section.BurnSection();

        if (section.HasHeart)
        {
            OnHeartEnd?.Invoke(section.ConnectLeft == null);
        }
    }

    public void BreakBarSections(int number, bool fromLeft, bool burn = false)
    {
        StartCoroutine(BreakBarSectionsCR(number, fromLeft, burn));
    }

    private IEnumerator BreakBarSectionsCR(int number, bool fromLeft, bool burn = false)
    {
        int startIndex = fromLeft ? 0 : _barSectionList.Count - 1;
        
        var sections = new List<BarSection>(_barSectionList);

        if (!fromLeft) sections.Reverse();

        for (int i = 0; i < number; i++)
        {
            if (i >= sections.Count) yield break;

            BreakBarSection(sections[i], burn);

            yield return new WaitForSeconds(0.5f);
        }

    }

    [Button("Break Bar Section Test")]
    public void RemoveBarSectionTest()
    {
        BreakBarSections(3, true);
    }

    // Hearth Variables
    private BarSection _currentHeartSection = null;
    public int CurrentHeartIndex => _barSectionList.IndexOf(_currentHeartSection);

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
        int index = 0;

        if (_sectionsNumber % 2 == 0)
        {
            int rnd = _random.Next(0, 1);

            index = (_sectionsNumber / 2) + rnd;
        }
        else
        {
            index = (_sectionsNumber / 2) + 1;
        }

        _currentHeartSection = _barSectionList[index];
        _currentHeartSection.SetHasHeart(true);

        _pullingUIManager.MoveHeart(index, doAnim: false);

    }

    // Moving Logic
    public bool IsMoving { get; private set; }
    private Coroutine _moveHeartCoroutine;

    public void MoveHeart(int pushForce, BattlerController user)
    {
        if (_moveHeartCoroutine != null) StopCoroutine(_moveHeartCoroutine);

        _moveHeartCoroutine = StartCoroutine(MoveHeartCR(Mathf.Abs(pushForce), pushForce > 0, user));
    }
    private IEnumerator MoveHeartCR(int pushForce, bool positive, BattlerController user)
    {
        IsMoving = true;

        for (int i = 0; i < pushForce; i++)
        {
            BarSection section = _currentHeartSection;

            // Check Modifier in Current Bar Section
            DoBarModifier(section, BarModifierTrigger.OnInside, user); 

            var nextSection = positive ? section.ConnectLeft : section.ConnectRight;

            if (nextSection != null)
            {
                // Check Modifier in Next Bar Section
                DoBarModifier(nextSection, BarModifierTrigger.OnEnter, user);

                if (!IsMoving) yield break; // If the modifier stopped the movement, exit the coroutine

                section.SetHasHeart(false);

                // Check Modifier in Exiting Bar Section
                DoBarModifier(section, BarModifierTrigger.OnExit, user);

                if (!IsMoving) yield break; // If the modifier stopped the movement, exit the coroutine

                nextSection.SetHasHeart(true);
                
                _currentHeartSection = nextSection;

                _pullingUIManager.MoveHeart(CurrentHeartIndex, () => OnSoulMove?.Invoke());
            }
            else if (positive)
            {
                // Tip over to the right
                _pullingUIManager.MoveHeartTipOver(true);
                OnHeartEnd?.Invoke(false);
                IsMoving = false;
                yield break;
            }
            else
            {
                // Tip over to the left
                _pullingUIManager.MoveHeartTipOver(false);
                OnHeartEnd?.Invoke(true);
                IsMoving = false;
                yield break;
            }

            yield return new WaitForSeconds(_pullSpeed);
        }

        IsMoving = false;
    }

    public void StopMovement()
    {
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

            if (section.BarModifier.CheckIfDone())
            {
                section.RemoveBarModifier();
            }
        }
    }
    public void DoBarModifier(BarSection section, BarModifierTrigger trigger, BattlerController user)
    {
        if (!section.HasModifier) return;

        if (section.BarModifier.Trigger != trigger) return;

        section.BarModifier.BarModifierLogic.OnBarModifierTriggered(_barSectionList.IndexOf(section), user, this);

        if (section.BarModifier.DestroyOnUse) section.RemoveBarModifier();
    }

    public void DoBarModifiers(BarModifierTrigger trigger)
    {
        foreach (BarSection section in _barSectionList)
        {
            DoBarModifier(section, trigger, null);
        }
    }

}
