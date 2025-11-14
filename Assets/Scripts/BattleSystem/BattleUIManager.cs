using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private FillBar _playerFillBar;
    [SerializeField] private FillBar _enemyFillBar;

    [SerializeField] private Transform _playerPivot;
    [SerializeField] private Transform _enemyPivot;

    [SerializeField] private GameObject _actionButtons;
    [SerializeField] private GameObject _moveButtons;

    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

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

        _playerFillBar.SetUpBar(player.Name, "Stance", player.MaxStance);

        // CHANGE THIS TO ACCOMODATE MORE PARTY MEMBERS
        CharacterInfo enemy = enemyParty.PartyMembers[0];

        _enemyFillBar.SetUpBar(enemy.Name, "Stance", enemy.MaxStance);

    }
    
    public void UpdateStanceBars(PartyInfo playerParty, PartyInfo enemyParty)
    {
        CharacterInfo player = playerParty.PartyMembers[0];

        _playerFillBar.UpdateFillAmout(player.CurrentStance);

        // CHANGE THIS TO ACCOMODATE MORE PARTY MEMBERS
        CharacterInfo enemy = enemyParty.PartyMembers[0];

        _enemyFillBar.UpdateFillAmout(enemy.CurrentStance);
    }
}
