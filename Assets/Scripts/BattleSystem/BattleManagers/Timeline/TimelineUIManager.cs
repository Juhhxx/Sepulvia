using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;
using System.Linq;
using System;
using TMPro;

public class TimelineUIManager : MonoBehaviour
{
    [Header("Current Turn Indicator")]
    [Space(5f)]
    [SerializeField] private TextMeshProUGUI _currentTurnIndicatorTMP;

    [Header("Outside Indicator")]
    [Space(5f)]
    [SerializeField] private GameObject _outsideIndicator;
    [SerializeField] private TextMeshProUGUI _outsideIndicatorTMP;

    [Header("Timeline Prefabs and Parents")]
    [Space(5f)]
    // Sections
    [SerializeField] private Image _timelineSection;
    [SerializeField] private Image _firstTimelineSection;
    [SerializeField] private Image _middleTimelineSection;
    [SerializeField] private Image _finalTimelineSection;
    [SerializeField] private Transform _timelineSectionParent;

    // Indicators
    [SerializeField] private Transform _timelineIndicatorsParent;
    [SerializeField] private RectTransform _upperIndicatorPrefab;
    [SerializeField] private RectTransform _middleIndicatorPrefab;
    [SerializeField] private RectTransform _lowerIndicatorPrefab;

    [Header("Timeline Size Parameters")]
    [Space(5f)]
    [OnValueChanged("BuildTimeline"), SerializeField, Range(0,50)] private int _timelineSize;
    [SerializeField]
    private List<TimelineSection> _timelineSections = new List<TimelineSection>();
    [Button(enabledMode: EButtonEnableMode.Always)]
    private void ClearSectionsList()
    {
        foreach (TimelineSection s in _timelineSections) if (s != null) Destroy(s.Section.gameObject);
        _timelineSections.Clear();
    }

    private PullingManager _pullManager;
    [SerializeField]private TimelineManager _timelineManager;

    private void Awake()
    {
        _pullManager = FindAnyObjectByType<PullingManager>();
        _timelineManager = FindAnyObjectByType<TimelineManager>();

        if (_pullManager != null)
        {
            _pullManager.OnAddModifier += AddBarModifierIndicator;
        }

        _currentTurnIndicatorTMP.text = "0";

        ClearSectionsList();
        BuildTimeline();
    }

    public void UpdateTurnIndicator()
    {
        _currentTurnIndicatorTMP.text = $"{_timelineManager.CurrentTurn}";
    }

    public void UpdateOutsideIndicator()
    {
        int num = _timelineIndicators.Count(i => i.Turn >= _timelineSize);

        if (num == 0)
        {
            _outsideIndicator.SetActive(false);
            return;
        }
        _outsideIndicator.SetActive(true);
        _outsideIndicatorTMP.text = $"+{num}";
    }

    public void BuildTimeline()
    {
        if (_timelineSection == null || _timelineSize < 0) return;

        Debug.Log("Building Timeline");

        for (int i = 0; i < _timelineSize; i++)
        {
            if (_timelineSections.Count >= i + 1)
            {
                _timelineSections[i].Section.gameObject.SetActive(true);
            }
            else
            {
                Image prefab = (i == 0) ? _firstTimelineSection : _timelineSection;

                // middle of timeline
                if (i == (_timelineSize/2)) prefab = _middleTimelineSection;

                // end of timeline
                if (i == _timelineSize - 1) prefab = _finalTimelineSection;

                Image newSection = Instantiate(prefab, _timelineSectionParent);

                _timelineSections.Add(new TimelineSection(newSection.rectTransform));
            }
        }

        if (_timelineSections.Count > _timelineSize)
        {
            for (int i = _timelineSize; i < _timelineSections.Count; i++)
            {
                Destroy(_timelineSections[i].Section);
            }
        }

    }

    [SerializeField]
    private List<TimelineIndicator> _timelineIndicators = new List<TimelineIndicator>();
    public void ClearIndicators()
    {
        foreach (TimelineIndicator ti in _timelineIndicators)
        {
            Destroy(ti.Indicator.gameObject);
        }
        foreach (TimelineSection ts in _timelineSections)
        {
            ts.Indicators.Clear();
        }

        _timelineIndicators.Clear();
    }

    public void AddPartyTimelineIndicators(Party playerParty, Party enemyParty)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_timelineSectionParent.GetComponent<RectTransform>());

        AddPartyTimelineIndicators(playerParty, TimelinePosition.Upper);
        AddPartyTimelineIndicators(enemyParty, TimelinePosition.Lower);
    }

    private void AddPartyTimelineIndicators(Party party, TimelinePosition position)
    {
        for (int i = 0; i < party.PartySize; i++)
        {
            Character c = party.PartyMembers[i];
            string name = c.Name + " " + i;

            AddTimelineIndicator(name, 0, c.TimelineIndicator, position);

            c.OnRecoveryTimeChange += (newT, oldT) =>
            {
                UpdateTimelineIndicator(name, oldT, newT);
            };
        }
    }

    public void AddBarModifierIndicator(int section, BarModifier barModifier)
    {
        string name = barModifier.Name + " " + section;

        AddTimelineIndicator(name, barModifier.TurnDuration, barModifier.TimelineIndicator, TimelinePosition.Middle);

        barModifier.OnTurnPassed += (newT, oldT) =>
        {
            UpdateTimelineIndicator(name, oldT, newT);
        };

        barModifier.OnCompleted += () =>
        {
            RemoveTimelineIndicator(name);
        };
    }
    public void AddBSoulBurnIndicator(SoulBurnProfile soulBurn)
    {
        string name = "Soul Burn";

        AddTimelineIndicator(name, soulBurn.TurnsUntil, soulBurn.Icon, TimelinePosition.Middle);

        soulBurn.OnTurnPassed += (newT, oldT) =>
        {
            UpdateTimelineIndicator(name, oldT, newT);
        };
    }

    public void AddTimelineIndicator(string name, int turn, Sprite sprite, TimelinePosition position)
    {
        if (_timelineIndicators.Any(i => i.Name == name)) return;

        RectTransform indicator = null;

        switch (position)
        {
            case TimelinePosition.Upper:

                indicator = Instantiate(_upperIndicatorPrefab, _timelineIndicatorsParent);
                break;
            
            case TimelinePosition.Middle:

                indicator = Instantiate(_middleIndicatorPrefab, _timelineIndicatorsParent);
                break;
            
            case TimelinePosition.Lower:

                indicator = Instantiate(_lowerIndicatorPrefab, _timelineIndicatorsParent);
                break;
        }

        Image image = indicator.GetComponentInChildren<Image>();

        image.sprite = sprite;
        indicator.gameObject.name = name;

        _timelineIndicators.Add(new TimelineIndicator(name, turn, indicator, image, position));

        if (turn >= _timelineSize) indicator.gameObject.SetActive(false);

        UpdateTimelineIndicator(name, turn, turn, false);
    }
    public void RemoveTimelineIndicator(string name)
    {
        var ti = _timelineIndicators.Find(i => i.Name == name);

        HideIndicator(ti.Indicator, () => Destroy(ti.Indicator.gameObject));

        _timelineIndicators.Remove(ti);
    }

    public void UpdateTimelineIndicator(string name, int fromTurn, int toTurn, bool doAnim = true)
    {
        TimelineIndicator timelineIndicator = _timelineIndicators.Find(i => i.Name == name);
        
        MoveIndicator(timelineIndicator, fromTurn, toTurn, doAnim);
    }

    [Header("Timeline Animations Parameters")]
    [Space(5f)]
    [SerializeField] private float _overlaySpacing = 35f;
    [SerializeField] private float _overlayAnimSpeed = 0.1f;
    [SerializeField] private Ease _overlayAnimEase = Ease.Linear;
    [SerializeField] private float _moveAnimSpeed = 0.5f;
    [SerializeField] private Ease _moveAnimEase = Ease.Linear;
    [SerializeField] private float _showHideAnimSpeed = 0.5f;
    [SerializeField] private Ease _showHideAnimEase = Ease.Linear;

    private void ResolveOverlay(TimelineSection section, TimelinePosition position)
    {
        var indicators = section.Indicators.FindAll(i => i.Position == position);
        int num = indicators.Count;

        for (int i = 0; i < indicators.Count; i++)
        {
            if (indicators[i]?.Indicator == null) continue;

            float yPos = _overlaySpacing * i;

            var timelineIndicator = indicators[i];

            if (yPos == timelineIndicator.Indicator.anchoredPosition.x) continue;

            if (position == TimelinePosition.Lower) yPos *= -1;

            timelineIndicator.Indicator.transform.SetSiblingIndex(num - i - 1);
            MoveIndicatorVertical(timelineIndicator.Indicator, yPos);
        }
        
    }
    private void MoveIndicator(TimelineIndicator timelineIndicator, int from, int to, bool doAnim = true)
    {
        RectTransform indicator = timelineIndicator.Indicator;

        indicator.gameObject.SetActive(true);
        // indicator.transform.SetAsLastSibling();

        Debug.Log($"Moving Indicator {timelineIndicator.Name} from section {from} to section {to}", this);

        if (from >= 0 && from < _timelineSize - 1)
        {
            _timelineSections[from]?.RemoveIndicator(timelineIndicator);
            ResolveOverlay(_timelineSections[from], timelineIndicator.Position);
        }

        if (to >= 0 && to < _timelineSize - 1)
            _timelineSections[to]?.AddIndicator(timelineIndicator);

        timelineIndicator.Turn = to;

        if (to >= 0 && to < _timelineSize)
        {
            var pos = _timelineSections[to].Section.anchoredPosition;
            pos.y = 0;

            if (doAnim)
            {
                if (from >= _timelineSize)
                {
                    ShowIndicator(indicator, () =>
                    MoveIndicatorHorizontal(indicator, pos.x, ()=>
                    ResolveOverlay(_timelineSections[to], timelineIndicator.Position)));
                }
                else
                {
                    MoveIndicatorHorizontal(indicator, pos.x, ()=>
                    ResolveOverlay(_timelineSections[to], timelineIndicator.Position));
                }
            }
            else
            {
                indicator.anchoredPosition = pos;
                ResolveOverlay(_timelineSections[to], timelineIndicator.Position);
            }
        }
        else if (to < 0)
        {
            var pos = _timelineSections[0].Section.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            indicator.anchoredPosition = pos;
        }
        else
        {
            // Indicator outside timeline
            var pos = _timelineSections.Last().Section.anchoredPosition;
            pos.x += + _timelineSections[0].Section.rect.width / 2;
            pos.y = indicator.anchoredPosition.y;

            if (doAnim)
            {
                MoveIndicatorHorizontal(indicator, pos.x, () =>
                HideIndicator(indicator));               
            }
            else
            {
                indicator.anchoredPosition = pos;
                indicator.gameObject.SetActive(false);
            }
        }

        UpdateOutsideIndicator();
    }
    private void MoveIndicatorHorizontal(RectTransform indicator, float xPos, Action onDone = null)
    {
        indicator.DOKill();
        indicator.DOAnchorPosX(xPos, _moveAnimSpeed)
                .SetEase(_moveAnimEase)
                .OnComplete(() => onDone?.Invoke());
    }
    private void MoveIndicatorVertical(RectTransform indicator, float yPos, Action onDone = null)
    {
        indicator.DOKill();
        indicator.DOAnchorPosY(yPos, _overlayAnimSpeed)
                .SetEase(_overlayAnimEase)
                .OnComplete(() => onDone?.Invoke());
    }

    private void HideIndicator(RectTransform indicator, Action onDone = null)
    {
        indicator.DOKill();
        indicator.DOScale(0f, _showHideAnimSpeed)
                .SetEase(_showHideAnimEase)
                .OnComplete(() => onDone?.Invoke());
    }
    private void ShowIndicator(RectTransform indicator, Action onDone = null)
    {
        indicator.localScale = Vector2.zero;

        indicator.DOKill();
        indicator.DOScale(1f, _showHideAnimSpeed)
                .SetEase(_showHideAnimEase)
                .OnComplete(() => onDone?.Invoke());
    }
}

[Serializable]
public class TimelineSection
{
    public RectTransform Section;
    public List<TimelineIndicator> Indicators => _indicators;
    [SerializeField] private List<TimelineIndicator> _indicators;

    public void AddIndicator(TimelineIndicator indicator)
    {
        if (!_indicators.Contains(indicator)) _indicators.Add(indicator);
    }
    public void RemoveIndicator(TimelineIndicator indicator)
    {
        _indicators.Remove(indicator);
    }

    public TimelineSection(RectTransform section)
    {
        Section = section;
        _indicators = new List<TimelineIndicator>();
    }
}

[Serializable]
public class TimelineIndicator
{
    public string Name;
    public int Turn;
    public RectTransform Indicator;
    public Image Image;
    public TimelinePosition Position;

    public TimelineIndicator(string name, int turn, RectTransform indicator, Image image, TimelinePosition position)
    {
        Name = name;
        Turn = turn;
        Indicator = indicator;
        Image = image;
        Position = position;
    }
}
public enum TimelinePosition
{
    Upper,
    Middle,
    Lower,
}
