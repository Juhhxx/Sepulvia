using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;
using System.Linq;

public class TimelineUIManager : MonoBehaviour
{
    [SerializeField] private Image _timelineSection;
    [SerializeField] private Transform _timelineSectionParent;
    [SerializeField] private Transform _timelineIndicatorsParent;

    [SerializeField] private RectTransform _upperIndicatorPrefab;
    [SerializeField] private RectTransform _middleIndicatorPrefab;
    [SerializeField] private RectTransform _lowerIndicatorPrefab;

    [OnValueChanged("BuildTimeline"), SerializeField, Range(0,50)] private int _timelineSize;
    private List<Image> _timelineSections = new List<Image>();
    [Button(enabledMode: EButtonEnableMode.Always)]
    private void ClearSectionsList()
    {
        foreach (Image i in _timelineSections) if (i != null) Destroy(i.gameObject);
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
                _timelineSections[i].gameObject.SetActive(true);
            }
            else
            {
                Image newSection = Instantiate(_timelineSection, _timelineSectionParent);
                _timelineSections.Add(newSection);
            }
        }

        if (_timelineSections.Count > _timelineSize)
        {
            for (int i = _timelineSize; i < _timelineSections.Count; i++)
            {
                Destroy(_timelineSections[i]);
            }
        }
    }

    private List<TimelineIndicator> _timelineIndicators = new List<TimelineIndicator>();
    public void ClearIndicators()
    {
        foreach (TimelineIndicator ti in _timelineIndicators)
        {
            Destroy(ti.Indicator.gameObject);
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

        _timelineIndicators.Add(new TimelineIndicator(name, turn, indicator));

        UpdateTimelineIndicator(name, 0, false);
    }

    public void UpdateTimelineIndicator(string name, int position, bool doAnim = true)
    {
        RectTransform indicator = _timelineIndicators.Find(i => i.Name == name).Indicator;

        MoveIndicator(indicator, position, doAnim);
    }
    private void MoveIndicator(RectTransform indicator, int position, bool doAnim = true)
    {
        indicator.gameObject.SetActive(true);
        indicator.transform.SetAsLastSibling();

        if (position >= 0 && position <= _timelineSize)
        {
            var pos = _timelineSections[position].rectTransform.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            if (doAnim)
            {
                indicator.DOKill();
                indicator.DOAnchorPosX(pos.x, 0.5f);
            }
            else
            {
                indicator.anchoredPosition = pos;
            }
        }
        else if (position < 0)
        {
            var pos = _timelineSections[0].rectTransform.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            indicator.anchoredPosition = pos;
        }
        else
        {
            indicator.gameObject.SetActive(false);
        }
    }
}

public struct TimelineIndicator
{
    public string Name;
    public int Turn;
    public RectTransform Indicator;

    public TimelineIndicator(string name, int turn, RectTransform indicator)
    {
        Name = name;
        Turn = turn;
        Indicator = indicator;
    }
}
public enum TimelinePosition
{
    Upper,
    Middle,
    Lower,
}
