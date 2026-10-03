using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetButtonManager : MonoBehaviour
{
    [SerializeField] private Button _targetButtonPrefab;
    [SerializeField] private Transform _targetButtonParent;
    [SerializeField] private Button _confirmButton;

    private BattlerController _player;
    private BattlerController[] _enemyParty;

    private List<BattlerController> _selectedEnemies = new List<BattlerController>();
    private int _numberOfTargets;

    private List<Button> _createdButtons = new List<Button>();

    private void Start()
    {
        _confirmButton.onClick.AddListener(() => SendTargetedEnemies());
    }

    public void RequestTargets(int number, BattlerController player, BattlerController[] enemyParty)
    {
        _player = player;
        _enemyParty = enemyParty;

        _numberOfTargets = number;
        _selectedEnemies.Clear();

        _confirmButton.interactable = _selectedEnemies.Count == _numberOfTargets;

        UpdateTargetButtons();
    }

    private int _maxPartySize = 3;
    private void UpdateTargetButtons()
    {
        if (_createdButtons.Count == 0)
        {
            for (int i = 0; i < _maxPartySize; i++)
            {
                Button button = Instantiate(_targetButtonPrefab, _targetButtonParent);
                _createdButtons.Add(button);
            }
        }

        for (int i = 0; i < _createdButtons.Count; i++)
        {
            Debug.Log($"BUTTON {i} PARTY SIZE {_enemyParty.Length}");

            if (i < _enemyParty.Length)
            {
                _createdButtons[i].gameObject.SetActive(true);
                UpdateTargetButton(_createdButtons[i], _enemyParty[i]);
            }
            else
            {
                _createdButtons[i].gameObject.SetActive(false);
            }
        }
        
    }
    private void UpdateTargetButton(Button b, BattlerController bc)
    {
        TargetButtonUI tbui = b.GetComponent<TargetButtonUI>();

        tbui.SetUp(bc.Character);

        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() =>
        {
            ToggleTarget(bc);
            tbui.ToggleSelect();
        });
    }

    private void SendTargetedEnemies()
    {
        foreach (BattlerController c in _selectedEnemies)
        {
            _player.AddTarget(c);
        }
    }

    private void ToggleTarget(BattlerController BattlerController)
    {
        if (_selectedEnemies.Contains(BattlerController))
        {
            _selectedEnemies.Remove(BattlerController);
        }
        else _selectedEnemies.Add(BattlerController);

        Debug.Log($"SELECTED ENEMIE {_selectedEnemies.Count} ENEMIES REQUESTED {_numberOfTargets}");

        _confirmButton.interactable = _selectedEnemies.Count == _numberOfTargets;
    }
}
