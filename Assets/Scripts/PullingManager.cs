using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;

public class PullingManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Spawn Settings")] Canvas canvas;
    [SerializeField, BoxGroup("Spawn Settings")] Image pullBar;
    [SerializeField, BoxGroup("Spawn Settings")] Image sectionBar;
    [SerializeField, BoxGroup("Spawn Settings"), Range(0, 1920), OnValueChanged("SpawnBarSections")] float barsTotalWidth;
    [SerializeField, BoxGroup("Spawn Settings"), Range(-540, 540), OnValueChanged("SpawnBarSections")] float barY = -410f;
    [SerializeField, BoxGroup("Spawn Settings"), OnValueChanged("SpawnBarSections")] int divNumb;
    [SerializeField, BoxGroup("Spawn Settings"), OnValueChanged("SpawnBarSections")] int padding;

    private List<GameObject> _spawnedObjects;

    [SerializeField] Image heart;
    private float heartHeightPadding = 100;
    public List<Vector3> heartPositions = new List<Vector3>();
    public List<Image> barSectionList = new List<Image>();
    private int currentHeartIndex;
    private Image spawnedHeart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        SpawnBarSections();
        Spawnheart();
        // foreach (Vector3 heartsy in heartPositions)
        // {
        //     Debug.Log(heartsy);
        // }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveHeart(-2);
            Debug.Log(currentHeartIndex);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            MoveHeart(-1);
            Debug.Log(currentHeartIndex);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            MoveHeart(1);
            Debug.Log(currentHeartIndex);

        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            MoveHeart(2);
            Debug.Log(currentHeartIndex);

        }
        
    }

    private void Spawnheart()
    {
        //Instantiate the heart Image prefab
        spawnedHeart = Instantiate(heart, canvas.transform);

        //Check if the number of bar sections is even or not to know where to spwn it

        if (heartPositions.Count() % 2 != 0) //it's not an even number of sections
        {
            //as it's uneven, it just spawns above the anchor point of the center bar
            currentHeartIndex = (int)Mathf.Ceil(heartPositions.Count() / 2);
            spawnedHeart.rectTransform.anchoredPosition = new Vector3(0, barY + heartHeightPadding, 0);
        }
        else
        {
            //if it's even then a coin toss is executed, spawning the heart on the bar either by the right or left of the center point
            int coinToss = Random.Range(0, 2);
            if (coinToss == 0)
            {
                currentHeartIndex = (int)Mathf.Ceil(heartPositions.Count() / 2) - 1;
                spawnedHeart.rectTransform.anchoredPosition = new Vector3(heartPositions[currentHeartIndex].x, barY + heartHeightPadding, 0);
            }
            else
            {
                currentHeartIndex = (int)Mathf.Ceil(heartPositions.Count() / 2) + 1;
                spawnedHeart.rectTransform.anchoredPosition = new Vector3(heartPositions[currentHeartIndex].x, barY + heartHeightPadding, 0);
            }
        }
    }
    [Button(enabledMode:EButtonEnableMode.Always)]
    private void SpawnBarSections()
    {
        if (canvas == null || pullBar == null || sectionBar == null) return;

        if (_spawnedObjects != null)
        {
            foreach (GameObject go in _spawnedObjects) DestroyImmediate(go);
            _spawnedObjects.Clear();
        }
        else _spawnedObjects = new List<GameObject>();

        heartPositions?.Clear();
        barSectionList?.Clear();
        
        if (divNumb <= 0) return;

        float sectionWidth;

        if (barsTotalWidth < padding)
        {
            padding = 0;
            sectionWidth = (barsTotalWidth - (padding * (divNumb - 1))) / divNumb;
        }
        else
        {
            sectionWidth = (barsTotalWidth - (padding * (divNumb - 1))) / divNumb;
        }

        for (int i = 0; i < divNumb; i++)
        {
            Image spawnedBar = Instantiate(sectionBar, canvas.transform);

            // Resize each section
            spawnedBar.rectTransform.sizeDelta = new Vector2(sectionWidth, spawnedBar.rectTransform.sizeDelta.y);

            // Anchor to middle
            spawnedBar.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            spawnedBar.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            spawnedBar.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Calculate position
            float x = -barsTotalWidth / 2f + (i * (sectionWidth + padding)) + (sectionWidth / 2f);
            spawnedBar.rectTransform.anchoredPosition = new Vector2(x, barY);

            //Add new bar position to a list that you can use as anchor points to move the heart around
            heartPositions.Add(new Vector3(spawnedBar.rectTransform.anchoredPosition.x, spawnedBar.rectTransform.anchoredPosition.y + heartHeightPadding, 0));
            //Add the each of the new spwaned bars to a list so that your can give them attributes
            barSectionList.Add(spawnedBar);

            _spawnedObjects.Add(spawnedBar.gameObject);
        }
    }

    public void MoveHeart(int pushForce)
    {
        //update the index position of the heart
        currentHeartIndex += pushForce;
        //visually move the heart to another anchor point
        spawnedHeart.rectTransform.anchoredPosition = heartPositions[currentHeartIndex];
    }
}
