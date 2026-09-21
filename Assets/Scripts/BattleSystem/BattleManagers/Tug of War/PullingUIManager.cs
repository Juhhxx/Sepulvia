using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class PullingUIManager : MonoBehaviour
{
    [Header("UI Parameters")]
    [SerializeField] Canvas _canvas;
    [SerializeField] Transform _pullBarParent;
    [SerializeField] GameObject _heartPrefab;
    [SerializeField] GameObject _pullBarPrefab;
    [SerializeField, Range(0, 1920), OnValueChanged("SpawnBarSections")] float _barsTotalWidth;
    [SerializeField, Range(-540, 540), OnValueChanged("SpawnBarSections")] float _barY = -410f;
    [SerializeField, OnValueChanged("SpawnBarSections")] int _divNumb;
    [SerializeField, OnValueChanged("SpawnBarSections")] int _padding;
    [SerializeField] private float _heartHeightPadding = 100f;
    private float _sectionWidth;
    
    [Header("UI Animation Parameters")]
    [SerializeField] private float _pullHeartAnimSpeed = 1f;
    [SerializeField] private Ease _pullHeartAnimEase = Ease.InOutElastic;
    [SerializeField] private float _defaultHeartAnimSpeed = 1f;
    [SerializeField] private float _defaultHeartAnimMove = 0.5f;
    [SerializeField] private Ease _defaultHeartAnimEase = Ease.InOutFlash;


    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private GameObject _spawnedHeart;
    private RectTransform _heartTrans;

    private List<BarSection> _barSectionList = new List<BarSection>();
    public List<BarSection> GetBarSections() => _barSectionList;

    public void TogglePullUI(bool onOff)
    {
        if (_canvas != null)
        {
            _canvas.gameObject.SetActive(onOff);
        }
    }

    public void SpawnHeart()
    {
        if (_spawnedHeart != null) return;

        _spawnedHeart = Instantiate(_heartPrefab, _canvas.transform);
        _heartTrans = _spawnedHeart.GetComponent<RectTransform>();

        DoHeartDefaultAnim();
    }
    public void MoveHeart(int position, Action action = null, bool doAnim = true)
    {
        if (doAnim)
        {
            DoHeartMoveAnim(_barSectionList[position].HeartPosition, action);
        }
        else _heartTrans.anchoredPosition = _barSectionList[position].HeartPosition;
        
    }
    public void MoveHeartTipOver(bool right, Action action = null)
    {
        if (right) DoHeartMoveAnim(_barSectionList.Last().HeartPosition + (Vector3.right * _sectionWidth), action);
        else DoHeartMoveAnim(_barSectionList.First().HeartPosition + (Vector3.left * _sectionWidth), action);
    }

    private BarSection _lastBar = null;
    
    public void SpawnBarSections(int sectionsNumber)
    {
        if (_canvas == null || _pullBarPrefab == null) return;

        foreach (GameObject go in _spawnedObjects) Destroy(go);
        _spawnedObjects.Clear();

        _barSectionList.Clear();
        
        if (_divNumb <= 0) return;

        _divNumb = sectionsNumber;

        float sectionWidth;

        if (_barsTotalWidth < _padding)
        {
            _padding = 0;
            sectionWidth = (_barsTotalWidth - (_padding * (_divNumb - 1))) / _divNumb;
        }
        else
        {
            sectionWidth = (_barsTotalWidth - (_padding * (_divNumb - 1))) / _divNumb;
        }

        for (int i = 0; i < _divNumb; i++)
        {
            GameObject spawnedBar = Instantiate(_pullBarPrefab, _pullBarParent);

            BarSection section = spawnedBar.GetComponent<BarSection>();

           _barSectionList.Add(section);

           if (_lastBar != null)
            {
                section.ConnectRight = _lastBar;
                _lastBar.ConnectLeft = section;
            }

            _lastBar = section;

            // Resize each section
            section.RectTransform.sizeDelta = new Vector2(sectionWidth, section.RectTransform.sizeDelta.y);

            // Anchor to middle
            section.RectTransform.pivot = new Vector2(0.5f, 0.5f);
            section.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            section.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Calculate position
            float x = -_barsTotalWidth / 2f + (i * (sectionWidth + _padding)) + (sectionWidth / 2f);
            section.RectTransform.anchoredPosition = new Vector2(x, _barY);

            section.SetHeartPosition(new Vector3(section.RectTransform.anchoredPosition.x, section.RectTransform.anchoredPosition.y + _heartHeightPadding, 0));

            _spawnedObjects.Add(spawnedBar.gameObject);
        }

        _sectionWidth = _barSectionList[0].RectTransform.sizeDelta.x;

    }

    public void ToggleBarButtons(bool onOff)
    {
        foreach (BarSection section in _barSectionList)
        {
            if (onOff)
            {
                if (!section.HasModifier && !section.HasHeart) section.Button.enabled = true;
                else section.Button.enabled = false;
            }
            else section.Button.enabled = false;
        }
    }

    // Animations
    public void DoHeartMoveAnim(Vector3 position, Action action)
    {
        _heartTrans.DOAnchorPosX(position.x, _pullHeartAnimSpeed).SetEase(_pullHeartAnimEase);
        CameraEffectsUtility.DoCameraShake(0.5f, 0.5f, delay: _pullHeartAnimSpeed / 2, action: action);
    }
    public void DoHeartDefaultAnim()
    {
        Vector3 pos = _heartTrans.anchoredPosition;

        _heartTrans.DOAnchorPosY(pos.y + _defaultHeartAnimMove, _defaultHeartAnimSpeed).SetEase(_defaultHeartAnimEase).SetLoops(-1, LoopType.Yoyo);
    }
}