using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private FillBar _playerStanceBar;
    [SerializeField] private GameObject _enemyStanceBarPrefab;
    private List<FillBar> _enemyStanceBars;

    [SerializeField] private Transform _playerPivot;
    [SerializeField] private Transform _enemyPivot;
    private List<GameObject> _characterModels;

    [SerializeField] private GameObject _actionButtons;
    [SerializeField] private GameObject _moveButtons;

    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    public void InstantiateBattlePrefabs(PartyInfo playerParty, PartyInfo enemyParty)
    {
        _characterModels = new List<GameObject>();

        CharacterInfo player = playerParty.PartyMembers[0];

        GameObject playerGO = Instantiate(player.BattlePrefab, _playerPivot.position, Quaternion.identity);

        _characterModels.Add(playerGO);

        foreach (CharacterInfo c in enemyParty.PartyMembers)
        {
            GameObject enemyGO = Instantiate(c.BattlePrefab, _enemyPivot.position, Quaternion.identity);
            _characterModels.Add(enemyGO);
        }
    }
    
    public void ToogleActionButtons(bool onOff) => _actionButtons.SetActive(onOff);
    public void ToogleMoveButtons(bool onOff) => _moveButtons.SetActive(onOff);

    public List<Button> GetActionButtons()
    => _actionButtons.GetComponentsInChildren<Button>().ToList();

    public List<Button> GetMoveButtons()
    => _moveButtons.transform.GetChild(0).GetComponentsInChildren<Button>().ToList();

    public void SetUpButton(Button button, string name)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = name;
    }

    public void SetUpStanceBars(PartyInfo playerParty, PartyInfo enemyParty)
    {        
        CharacterInfo player = playerParty.PartyMembers[0];

        _playerStanceBar.SetUpBar(player.Name, "Stance", player.MaxStance);

        _enemyStanceBars = new List<FillBar>();

        foreach (CharacterInfo enemy in enemyParty.PartyMembers)
        {
            Vector3 pos = _characterModels[enemyParty.PartyMembers.IndexOf(enemy) + 1].transform.position;

            GameObject bar = Instantiate(_enemyStanceBarPrefab, gameObject.transform);
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
}
