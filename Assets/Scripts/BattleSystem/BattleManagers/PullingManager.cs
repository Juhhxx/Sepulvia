using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public class PullingManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Spawn Settings")] Canvas _canvas;
    [SerializeField, BoxGroup("Spawn Settings")] GameObject _pullBar;
    [SerializeField, BoxGroup("Spawn Settings")] GameObject _sectionBar;
    [SerializeField, BoxGroup("Spawn Settings"), Range(0, 1920), OnValueChanged("SpawnBarSections")] float _barsTotalWidth;
    [SerializeField, BoxGroup("Spawn Settings"), Range(-540, 540), OnValueChanged("SpawnBarSections")] float _barY = -410f;
    [SerializeField, BoxGroup("Spawn Settings"), OnValueChanged("SpawnBarSections")] int _divNumb;
    [SerializeField, BoxGroup("Spawn Settings"), OnValueChanged("SpawnBarSections")] int _padding;

    private List<GameObject> _spawnedObjects;

    [SerializeField] GameObject _heart;
    [SerializeField] private float _pullSpeed = 0.2f;
    private float _heartHeightPadding = 100;
    [SerializeField, ReadOnly] private List<BarSection> _barSectionList = new List<BarSection>();
    public List<BarSection> BarSections => _barSectionList;

    private int _currentHeartIndex;
    private GameObject _spawnedHeart;
    [field: SerializeField, ReadOnly] public int SelectedIndex { get; private set; }
    public void SetSelectedIndex(int index)
    {
        SelectedIndex = index;
        OnSelectBar?.Invoke();
    }

    public event Action OnSelectBar;

    public event Action<bool> OnHeartEnd;

    public void ResetEvents()
    {
        OnSelectBar = null;
        OnHeartEnd = null;
    }

    public bool IsMoving { get; private set; }

    public void SpawnHeart()
    {
        if (_spawnedHeart != null) return;

        //Instantiate the heart Image prefab
        _spawnedHeart = Instantiate(_heart, _canvas.transform);
    }

    public void SetHeartInMiddle()
    {
        Image hearthImage = _spawnedHeart.GetComponent<Image>();

        //Check if the number of bar sections is even or not to know where to spwn it

        if (_barSectionList.Count() % 2 != 0) //it's not an even number of sections
        {
            //as it's uneven, it just spawns above the anchor point of the center bar
            _currentHeartIndex = (int)Mathf.Ceil(_barSectionList.Count() / 2);
            hearthImage.rectTransform.anchoredPosition = new Vector3(0, _barY + _heartHeightPadding, 0);
        }
        else
        {
            //if it's even then a coin toss is executed, spawning the _heart on the bar either by the right or left of the center point
            int coinToss =  UnityEngine.Random.Range(0, 2);
            if (coinToss == 0)
            {
                _currentHeartIndex = (int)Mathf.Ceil(_barSectionList.Count() / 2) - 1;
                hearthImage.rectTransform.anchoredPosition = new Vector3(_barSectionList[_currentHeartIndex].HeartPosition.x, _barY + _heartHeightPadding, 0);
            }
            else
            {
                _currentHeartIndex = (int)Mathf.Ceil(_barSectionList.Count() / 2) + 1;
                hearthImage.rectTransform.anchoredPosition = new Vector3(_barSectionList[_currentHeartIndex].HeartPosition.x, _barY + _heartHeightPadding, 0);
            }
        }
    }

    private BarSection _lastBar = null;
    
    public void SpawnBarSections(int sectionsNumber)
    {
        if (_canvas == null || _pullBar == null || _sectionBar == null) return;

        if (_spawnedObjects != null)
        {
            foreach (GameObject go in _spawnedObjects) DestroyImmediate(go);
            _spawnedObjects.Clear();
        }
        else _spawnedObjects = new List<GameObject>();

        _barSectionList?.Clear();
        
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
            GameObject spawnedBar = Instantiate(_sectionBar, _canvas.transform);

            Image image = spawnedBar.GetComponent<Image>();
            Button button = spawnedBar.GetComponent<Button>();
            BarSection section = spawnedBar.GetComponent<BarSection>();

            if (_lastBar != null)
            {
                section.ConnectRight = _lastBar;
                _lastBar.ConnectLeft = section;
            }

            _lastBar = section;
            

            int idx = i;
            button.onClick.AddListener(() => SetSelectedIndex(idx));
            button.enabled = false;

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

            _barSectionList.Add(section);

            _spawnedObjects.Add(spawnedBar.gameObject);
        }

    }

    public void ToggleBarButtons(bool onOff)
    {
        foreach (BarSection section in _barSectionList)
        {
            if (onOff)
            {
                if (!section.HasModifier) section.Button.enabled = true;
                else section.Button.enabled = false;
            }
            else section.Button.enabled = false;
        }
    }
    public void CheckBarModifiers()
    {
        foreach (BarSection section in _barSectionList)
        {
            if (section == null) continue;
            if (!section.HasModifier) continue;

            section.BarModifier.TurnPassed();

            if (section.BarModifier.CheckIfDone()) section.RemoveBarModifier();
        }
    }
    public bool DoBarModifier(BarSection section)
    {
        bool stopMovement = false;

        switch (section.BarModifier.Type)
        {
            case BarModifierTypes.Barrier:

                stopMovement = true;
                DialogueManager.Instance.AddDialogue($"A Barrier Stopped the Movement.");
                break;
        }

        if (section.BarModifier.DestroyOnUse) section.RemoveBarModifier();

        return stopMovement;
    }

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

            float sectionWidth = section.Image.rectTransform.sizeDelta.x;

            var tmp = _currentHeartIndex;

            if (positive) tmp++;
            else tmp--;

            if (tmp < 0)
            {
                _spawnedHeart.GetComponent<RectTransform>().anchoredPosition = section.HeartPosition + (Vector3.left * sectionWidth);
                OnHeartEnd?.Invoke(true);
                IsMoving = false;
                yield break;
            }
            else if (tmp >= _barSectionList.Count)
            {
                _spawnedHeart.GetComponent<RectTransform>().anchoredPosition = section.HeartPosition + (Vector3.right * sectionWidth);;
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

                section = _barSectionList[_currentHeartIndex];

                _spawnedHeart.GetComponent<RectTransform>().anchoredPosition = section.HeartPosition;
                section.SetHasHeart(true);
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
}
