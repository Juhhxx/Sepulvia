using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private FillBar _playerStanceBar;
    [SerializeField] private StatModifierDisplay _statModifierDisplay;
    [SerializeField] private GameObject _enemyStanceBarPrefab;
    private List<FillBar> _enemyStanceBars;

    [SerializeField] private Transform _playerPivot;
    [SerializeField] private Transform _enemyPivot;
    private List<GameObject> _characterModels;

    [SerializeField] private GameObject _actionButtons;
    [SerializeField] private GameObject _moveButtons;

    [SerializeField] private GameObject _seletBarCanvas;

    [SerializeField] private GameObject _turnOrderIndicator;

    [SerializeField] private GameObject _moveInfoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    [SerializeField] private GameObject _endBattleScreen;
    [SerializeField] private TextMeshProUGUI _endBattleText;

    public void ClearCreatedObjects()
    {
        if (_characterModels != null)
        {
            foreach (GameObject go in _characterModels) Destroy(go);
            _characterModels.Clear();
        }
       
        if (_enemyStanceBars != null)
        {
            foreach (FillBar f in _enemyStanceBars) Destroy(f.gameObject);
            _enemyStanceBars.Clear();
        }
    }

    public void InstantiateBattlePrefabs(PartyInfo playerParty, PartyInfo enemyParty)
    {
        _characterModels = new List<GameObject>();

        CharacterInfo player = playerParty.PartyMembers[0];

        GameObject playerGO = Instantiate(player.BattlePrefab, _playerPivot.position, Quaternion.identity);

        _characterModels.Add(playerGO);

        List<Vector3> positions = GetSpawnPoints(enemyParty.PartySize, _enemyPivot);

        for (int i = 0; i < enemyParty.PartySize; i++)
        {
            CharacterInfo c = enemyParty.PartyMembers[i];

            GameObject enemyGO = Instantiate(c.BattlePrefab, positions[i], Quaternion.identity);
            _characterModels.Add(enemyGO);
        }
    }

    private float radius = 4f;
    public List<Vector3> GetSpawnPoints(int count, Transform pivot)
    {
        List<Vector3> points = new List<Vector3>();

        if (count <= 0)
            return points;

        // Single point = just the pivot
        if (count == 1)
        {
            points.Add(GetPos(Vector3.zero, pivot));
            return points;
        }

        // Two points = vertical line up/down from pivot
        if (count == 2)
        {
            points.Add(GetPos(Vector3.right * radius, pivot));
            points.Add(GetPos(Vector3.left * radius, pivot));
            return points;
        }

        // 3+ = semi-circle facing forward (+Z)
        float angleStep = 180f / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angle = -90f + (angleStep * i); // from -90° to +90°

            // Rotation on Y axis
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            points.Add(GetPos(dir * radius, pivot));
        }

        return points;
    }

    private bool useLocalSpace;
    private Vector3 GetPos(Vector3 offset, Transform pivot)
    {
        return useLocalSpace ? pivot.TransformPoint(offset) : pivot.position + offset;
    }
    
    public void ToggleActionButtons(bool onOff) => _actionButtons.SetActive(onOff);
    public void ToggleMoveButtons(bool onOff) => _moveButtons.SetActive(onOff);
    public void ToggleSelecBar(bool onOff) => _seletBarCanvas.SetActive(onOff);

    public void ShowEndScreen(bool won)
    {
        if (won) _endBattleText.text = "YOU WON!";
        else _endBattleText.text = "YOU LOST!";

        _endBattleScreen.SetActive(true);
    }

    public List<Button> GetActionButtons()
    => _actionButtons.GetComponentsInChildren<Button>().ToList();

    public List<Button> GetMoveButtons()
    => _moveButtons.transform.GetChild(0).GetComponentsInChildren<Button>().ToList();

    public void SetUpButton(Button button, MoveInfo move)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = move.Name;

        button.GetComponent<MoveHoverInfo>().SetUpHover(move, this);
    }

    public void UpdateButton(Button button, bool activated)
    {
        if (activated)
        {
            button.enabled = true;
            button.GetComponent<Image>().color = Color.white;
        }
        else
        {
            button.enabled = false;
            Color transparent = Color.red;
            transparent.a = 0.5f;

            button.GetComponent<Image>().color = transparent;
        }
    }

    public void SetUpStanceBars(PartyInfo playerParty, PartyInfo enemyParty)
    {        
        CharacterInfo player = playerParty.PartyMembers[0];

        _playerStanceBar.SetUpBar(player.Name, "Stance", player.MaxStance);

        _enemyStanceBars = new List<FillBar>();

        foreach (CharacterInfo enemy in enemyParty.PartyMembers)
        {
            Vector3 pos = _characterModels[enemyParty.PartyMembers.IndexOf(enemy) + 1].transform.position;

            GameObject bar = Instantiate(_enemyStanceBarPrefab, pos + (Vector3.up * 7), Quaternion.identity);
            FillBar enemyFillBar = bar.GetComponent<FillBar>();

            _enemyStanceBars.Add(enemyFillBar);
            enemyFillBar.SetUpBar(enemy.Name, "Stance", enemy.MaxStance);
        }
    }
    public void UpdateStanceBars(PartyInfo playerParty, PartyInfo enemyParty)
    {
        CharacterInfo player = playerParty.PartyMembers[0];

        _playerStanceBar.UpdateFillAmout(player.CurrentStance);

        for (int i = 0; i < enemyParty.PartySize; i++)
        {
            _enemyStanceBars[i].UpdateFillAmout(enemyParty.PartyMembers[i].CurrentStance);
        }
    }
    
    public void UpdateStatModifierDisplay(List<StatModifier> statModifiers)
    {
        _statModifierDisplay.UpdateDisplay(statModifiers);
    }

    private List<GameObject> _createdObjectsTurns = new List<GameObject>();
    public void ShowTurnOrder(PartyInfo playerParty, PartyInfo enemyParty)
    {
        if (_createdObjectsTurns.Count > 0)
        {
            foreach (GameObject go in _createdObjectsTurns) Destroy(go);
            _createdObjectsTurns.Clear();
        }

        _turnOrderIndicator.transform.parent.gameObject.SetActive(true);

        TextMeshProUGUI prefab = _turnOrderIndicator.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        var battlers = new List<CharacterInfo>(playerParty.PartyMembers);

        battlers.AddRange(enemyParty.PartyMembers);

        battlers.Sort((a, b) => b.Speed.CompareTo(a.Speed));

        foreach (CharacterInfo c in battlers)
        {
            TextMeshProUGUI tmp = Instantiate(prefab, _turnOrderIndicator.transform);

            tmp.text = c.Name;

            tmp.gameObject.SetActive(true);

            _createdObjectsTurns.Add(tmp.gameObject);
        }
    }

    public void HideTurnOrder()
    {
        _turnOrderIndicator.transform.parent.gameObject.SetActive(false);
    }

    public void ToggleMoveInfo(bool onOff, MoveInfo move = null)
    {
        if (onOff)
        {
            _panelTitle.text = $"{move.Name}";
            _panelDescription.text = $"Cost: {move.StanceCost} stance\nCooldown: {move.Cooldown} turn(s)\n\n{move.Description}";

            if (move.CheckIfCooldown()) _panelTitle.text += $" (cooldown)";
        }
        
        _moveInfoPanel.SetActive(onOff);
    }
}
