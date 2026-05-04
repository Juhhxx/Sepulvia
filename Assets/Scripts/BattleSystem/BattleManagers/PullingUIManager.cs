using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class PullingUIManager : MonoBehaviour
{
    [Header("UI Parameters")]
    [SerializeField] Canvas _canvas;
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
            if (position < 0) DoHeartMoveAnim(_barSectionList[position + 1].HeartPosition + (Vector3.left * _sectionWidth), action);
            else if (position > _barSectionList.Count) DoHeartMoveAnim(_barSectionList[position - 1].HeartPosition + (Vector3.right * _sectionWidth), action);
            else DoHeartMoveAnim(_barSectionList[position].HeartPosition, action);
        }
        else _heartTrans.anchoredPosition = _barSectionList[position].HeartPosition;
        
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
            GameObject spawnedBar = Instantiate(_pullBarPrefab, _canvas.transform);

            Image image = spawnedBar.GetComponent<Image>();
            BarSection section = spawnedBar.GetComponent<BarSection>();

           _barSectionList.Add(section);

           if (_lastBar != null)
            {
                section.ConnectRight = _lastBar;
                _lastBar.ConnectLeft = section;
            }

            _lastBar = section;

            // Resize each section
            image.rectTransform.sizeDelta = new Vector2(sectionWidth, image.rectTransform.sizeDelta.y);

            // Anchor to middle
            image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Calculate position
            float x = -_barsTotalWidth / 2f + (i * (sectionWidth + _padding)) + (sectionWidth / 2f);
            image.rectTransform.anchoredPosition = new Vector2(x, _barY);

            section.SetTransform(image.rectTransform);

            //Add new bar position to a list that you can use as anchor points to move the _heart around
            section.SetHeartPosition(new Vector3(image.rectTransform.anchoredPosition.x, image.rectTransform.anchoredPosition.y + _heartHeightPadding, 0));

            _spawnedObjects.Add(spawnedBar.gameObject);
        }

        _sectionWidth = _barSectionList[0].Image.rectTransform.sizeDelta.x;

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
        CameraEffectsUtility.DoCameraShake(0.5f, 0.5f, _pullHeartAnimSpeed / 2, action);
    }
    public void DoHeartDefaultAnim()
    {
        Vector3 pos = _heartTrans.anchoredPosition;

        _heartTrans.DOAnchorPosY(pos.y + _defaultHeartAnimMove, _defaultHeartAnimSpeed).SetEase(_defaultHeartAnimEase).SetLoops(-1, LoopType.Yoyo);
    }
}