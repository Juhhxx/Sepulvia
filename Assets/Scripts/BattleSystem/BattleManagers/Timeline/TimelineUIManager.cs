using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;
using System.Linq;
using System;

public class TimelineUIManager : MonoBehaviour
{
    [SerializeField] private Image _timelineSection;
    [SerializeField] private Transform _timelineSectionParent;
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
                UpdateTimelineIndicator(name, c.RecoveryTime);
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

        UpdateTimelineIndicator(name, 0, false);
    }

    public void UpdateTimelineIndicator(string name, int turn, bool doAnim = true)
    {
        TimelineIndicator timelineIndicator = _timelineIndicators.Find(i => i.Name == name);

        _timelineSections[timelineIndicator.Turn]?.Indicators.Remove(timelineIndicator);
        _timelineSections[turn].Indicators.Add(timelineIndicator);

        timelineIndicator.Turn = turn;
        
        RectTransform indicator = timelineIndicator.Indicator;
        TimelinePosition position = timelineIndicator.Position;

        MoveIndicator(indicator, turn, position, doAnim);
    }
    private void MoveIndicator(RectTransform indicator, int turn, TimelinePosition position, bool doAnim = true)
    {
        if (indicator.anchoredPosition.y != 0)
        {
            indicator.DOAnchorPosY(0, 0.5f).OnComplete(() => MoveIndicator(indicator, turn, position));
            return;
        }

        indicator.gameObject.SetActive(true);
        indicator.transform.SetAsLastSibling();
        

        if (turn >= 0 && turn <= _timelineSize)
        {
            var pos = _timelineSections[turn].Section.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            if (doAnim)
            {
                indicator.DOKill();
                indicator.DOAnchorPosX(pos.x, 0.5f)
                        .OnComplete(() => ResolveOverlay(_timelineSections[turn], position));
            }
            else
            {
                indicator.anchoredPosition = pos;
            }
        }
        else if (turn < 0)
        {
            var pos = _timelineSections[0].Section.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            indicator.anchoredPosition = pos;
        }
        else
        {
            indicator.gameObject.SetActive(false);
        }
    }

    float overlaySpacing = 50f;
    private void ResolveOverlay(TimelineSection section, TimelinePosition position)
    {
        var indicators = section.Indicators.FindAll(i => i.Position == position);

        if (indicators.Count == 1) return;

        for (int i = 0; i < indicators.Count; i++)
        {
            float yPos = overlaySpacing * i;

            var timelineIndicator = indicators[i];


            if (yPos == 0 && timelineIndicator.Indicator.anchoredPosition.x == 0) continue;

            if (position == TimelinePosition.Lower) yPos *= -1;

            timelineIndicator.Indicator.transform.SetAsFirstSibling();
            timelineIndicator.Indicator.DOAnchorPosY(yPos, 0.1f);
        }
        
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
