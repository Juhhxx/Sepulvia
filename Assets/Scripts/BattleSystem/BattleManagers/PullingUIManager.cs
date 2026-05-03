using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
using System.Collections.Generic;

public class PullingUIManager : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _heartPrefab;
    [SerializeField] GameObject _pullBarPrefab;
    [SerializeField, Range(0, 1920), OnValueChanged("SpawnBarSections")] float _barsTotalWidth;
    [SerializeField, Range(-540, 540), OnValueChanged("SpawnBarSections")] float _barY = -410f;
    [SerializeField, OnValueChanged("SpawnBarSections")] int _divNumb;
    [SerializeField, OnValueChanged("SpawnBarSections")] int _padding;
    private float _heartHeightPadding = 100;

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private GameObject _spawnedHeart;

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
    }
    public void MoveHeart(int posotion)
    {
        _spawnedHeart.GetComponent<RectTransform>().anchoredPosition = _barSectionList[posotion].HeartPosition;
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
}