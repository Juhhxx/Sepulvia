using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private FillBar _playerFillBar;
    [SerializeField] private FillBar _enemyFillBar;
    
    [SerializeField] private Transform _playerPivot;
    [SerializeField] private Transform _enemyPivot;
}
