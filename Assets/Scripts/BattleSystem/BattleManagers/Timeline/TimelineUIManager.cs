using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;

public class TimelineUIManager : MonoBehaviour
{
    [SerializeField] private Image _timelineSection;
    [SerializeField] private Transform _timelineSectionParent;

    [SerializeField] private RectTransform _playerIndicator;
    [SerializeField] private RectTransform _enemyIndicator;

    [OnValueChanged("BuildTimeline"), SerializeField, Range(0,50)] private int _timelineSize;
    private List<Image> _timelineSections = new List<Image>();
    [Button(enabledMode: EButtonEnableMode.Always)]
    private void ClearSectionsList()
    {
        foreach (Image i in _timelineSections) if (i != null) Destroy(i.gameObject);
        _timelineSections.Clear();
    }

    private void Start()
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

    public void UpdateTimelineIndicators(int playerPosition, int enemyPosition)
    {
        Debug.Log($"pos player: {playerPosition}, pos ene: {enemyPosition}");
        MoveIndicator(_playerIndicator, playerPosition);
        MoveIndicator(_enemyIndicator, enemyPosition);
    }

    private void MoveIndicator(RectTransform indicator, int position)
    {
        if (position > 0 && position < _timelineSize)
        {
            var pos = _timelineSections[position].rectTransform.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            indicator.DOKill();
            indicator.DOAnchorPosX(pos.x, 0.5f);
        }
        else if (position == 0)
        {
            var pos = _timelineSections[position].rectTransform.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            indicator.anchoredPosition = pos;
        }
        else
        {
            var pos = _timelineSections[_timelineSize - 1].rectTransform.anchoredPosition;
            pos.y = indicator.anchoredPosition.y;

            indicator.anchoredPosition = pos;
        }
    }
}
