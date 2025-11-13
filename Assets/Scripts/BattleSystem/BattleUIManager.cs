using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private FillBar _playerFillBar;
    [SerializeField] private FillBar _enemyFillBar;

    [SerializeField] private Transform _playerPivot;
    [SerializeField] private Transform _enemyPivot;

    [SerializeField] private GameObject _actionButtons;
    [SerializeField] private GameObject _moveButtons;

    public void ToogleActionButtons(bool onOff) => _actionButtons.SetActive(onOff);
    public void ToogleMoveButtons(bool onOff) => _moveButtons.SetActive(onOff);

    
}
