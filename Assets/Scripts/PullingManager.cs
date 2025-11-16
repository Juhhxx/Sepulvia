using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;

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
    private float _heartHeightPadding = 100;
    [SerializeField] private List<BarSection> _barSectionList = new List<BarSection>();
    private int _currentHeartIndex;
    private GameObject _spawnedHeart;

    void Start()
    {
        
        SpawnBarSections();
        SpawnHeart();
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveHeart(-2);
            Debug.Log(_currentHeartIndex);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            MoveHeart(-1);
            Debug.Log(_currentHeartIndex);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            MoveHeart(1);
            Debug.Log(_currentHeartIndex);

        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            MoveHeart(2);
            Debug.Log(_currentHeartIndex);

        }
        
    }

    private void SpawnHeart()
    {
        //Instantiate the heart Image prefab
        _spawnedHeart = Instantiate(_heart, _canvas.transform);

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
            int coinToss = Random.Range(0, 2);
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

        _barSectionList[_currentHeartIndex].SetHasHeart(true);
    }
    [Button(enabledMode:EButtonEnableMode.Always)]
    private void SpawnBarSections()
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

            // Resize each section
            image.rectTransform.sizeDelta = new Vector2(sectionWidth, image.rectTransform.sizeDelta.y);

            // Anchor to middle
            image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Calculate position
            float x = -_barsTotalWidth / 2f + (i * (sectionWidth + _padding)) + (sectionWidth / 2f);
            image.rectTransform.anchoredPosition = new Vector2(x, _barY);

            BarSection section = spawnedBar.GetComponent<BarSection>();
            section.SetTransform(image.rectTransform);

            //Add new bar position to a list that you can use as anchor points to move the _heart around
            section.SetHeartPosition(new Vector3(image.rectTransform.anchoredPosition.x, image.rectTransform.anchoredPosition.y + _heartHeightPadding, 0));

            _barSectionList.Add(section);

            _spawnedObjects.Add(spawnedBar.gameObject);
        }
    }

    public void MoveHeart(int pushForce)
    {
        _barSectionList[_currentHeartIndex].SetHasHeart(false);
        _currentHeartIndex += pushForce;

        _spawnedHeart.GetComponent<RectTransform>().anchoredPosition = _barSectionList[_currentHeartIndex].HeartPosition;
        _barSectionList[_currentHeartIndex].SetHasHeart(true);
    }
}
