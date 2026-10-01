using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;
using System.Linq;
using System;

public class TimelineUIManager : MonoBehaviour
{
    [Header("Timeline Prefabs and Parents")]
    [Space(5f)]
    // Sections
    [SerializeField] private Image _timelineSection;
    [SerializeField] private Transform _timelineSectionParent;

    // Inidcators
    [SerializeField] private Transform _timelineIndicatorsParent;
    [SerializeField] private RectTransform _upperIndicatorPrefab;
    [SerializeField] private RectTransform _middleIndicatorPrefab;
    [SerializeField] private RectTransform _lowerIndicatorPrefab;

    [OnValueChanged("BuildTimeline"), SerializeField, Range(0,50)] private int _timelineSize;
    private List<TimelineSection> _timelineSections = new List<TimelineSection>();
    [Button(enabledMode: EButtonEnableMode.Always)]
    private void ClearSectionsList()
    {
        foreach (TimelineSection s in _timelineSections) if (s != null) Destroy(s.Section.gameObject);
        _timelineSections.Clear();
    }

    private void Awake()
    {
        ClearSectionsList();
        BuildTimeline();
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
                Image newSection = Instantiate(_timelineSection, _timelineSectionParent);

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
            _timelineSections[ti.Turn].Indicators.Remove(ti);
        }

        _timelineIndicators.Clear();
    }

    public void AddPartyTimelineIndicators(Party playerParty, Party enemyParty)
    {
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

        indicator.GetComponentInChildren<Image>().sprite = sprite;

        indicator.gameObject.name = name;

        _timelineIndicators.Add(new TimelineIndicator(name, turn, indicator, position));

        UpdateTimelineIndicator(name, 0, 0, false);
    }

    public void UpdateTimelineIndicator(string name, int fromTurn, int toTurn, bool doAnim = true)
    {
        TimelineIndicator timelineIndicator = _timelineIndicators.Find(i => i.Name == name);

        _timelineSections[fromTurn]?.Indicators.Remove(timelineIndicator);
        _timelineSections[toTurn].Indicators.Add(timelineIndicator);
        timelineIndicator.Turn = toTurn;
        
        RectTransform indicator = timelineIndicator.Indicator;
        TimelinePosition position = timelineIndicator.Position;
        Vector2 moveTo = Vector2.zero;

        moveTo.x = _timelineSections[toTurn].Section.anchoredPosition.x;
        moveTo.y = GetOverlayY(_timelineSections[toTurn], position); 

        MoveIndicator(indicator, moveTo, position, doAnim);
    }
    private float GetOverlayY(TimelineSection section, TimelinePosition position)
    {
        var indicators = section.Indicators.FindAll(i => i.Position == position);

        return _overlaySpacing * (indicators.Count - 1);
    }

    [SerializeField] private float _overlaySpacing = 35f;
    [SerializeField] private float _overlayAnimSpeed = 0.1f;
    [SerializeField] private Ease _overlayAnimEase = Ease.Linear;
    [SerializeField] private float _moveAnimSpeed = 0.5f;
    [SerializeField] private Ease _moveAnimEase = Ease.Linear;

    private void MoveIndicator(RectTransform indicator, Vector2 to, TimelinePosition position, bool doAnim = true)
    {
        indicator.gameObject.SetActive(true);

        if (indicator.anchoredPosition == to) return;
        else if (indicator.anchoredPosition.x > _timelineSections.Last().Section.anchoredPosition.x)
        {
            indicator.gameObject.SetActive(false); // Indicator outside timeline
        }
        
        if (doAnim)
        {
            if (to.y == indicator.anchoredPosition.y)
            {
                MoveIndicatorHorizontal(indicator, to.x);
            }
            else if (to.y > indicator.anchoredPosition.y)
            {
                if (position == TimelinePosition.Lower) to.y *= -1;

                MoveIndicatorHorizontal(indicator, to.x, () => 
                MoveIndicatorVertical(indicator, to.y, position, false));
            }
            else if (to.y < indicator.anchoredPosition.y)
            {
                if (position == TimelinePosition.Lower) to.y *= -1;

                MoveIndicatorVertical(indicator, to.y, position, true, ()=>
                MoveIndicatorHorizontal(indicator, to.x));
            }
        }
        else
        {
            if (position == TimelinePosition.Lower) to.y *= -1;
            
            indicator.anchoredPosition = to;
        }
    }
    private void MoveIndicatorHorizontal(RectTransform indicator, float xPos, Action onDone = null)
    {
        indicator.transform.SetAsLastSibling();

        indicator.DOKill();
        indicator.DOAnchorPosX(xPos, _moveAnimSpeed)
                .SetEase(_moveAnimEase)
                .OnComplete(() => onDone?.Invoke());
    }
    private void MoveIndicatorVertical(RectTransform indicator, float yPos, TimelinePosition position, bool inFront, Action onDone = null)
    {
        if (inFront) indicator.transform.SetAsLastSibling();
        else indicator.transform.SetAsFirstSibling();

        indicator.DOKill();
        indicator.DOAnchorPosY(yPos, _overlayAnimSpeed)
                .SetEase(_overlayAnimEase)
                .OnComplete(() => onDone?.Invoke());
    }
}

public class TimelineSection
{
    public RectTransform Section;
    public List<TimelineIndicator> Indicators;

    public TimelineSection(RectTransform section)
    {
        Section = section;
        Indicators = new List<TimelineIndicator>();
    }
}

[Serializable]
public class TimelineIndicator
{
    public string Name;
    public int Turn;
    public RectTransform Indicator;
    public TimelinePosition Position;

    public TimelineIndicator(string name, int turn, RectTransform indicator, TimelinePosition position)
    {
        Name = name;
        Turn = turn;
        Indicator = indicator;
        Position = position;
    }
}
public enum TimelinePosition
{
    Upper,
    Middle,
    Lower,
}
