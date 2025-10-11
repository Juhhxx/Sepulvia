using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName= "New party Member")]
public class PartyMemeberInfo : ScriptableObject
{
    public string MemberName;
    public int StartingLevel;
    public int BaseHealth;
    public int BaseStr;
    public int BaseInitiative;
    public GameObject MemberBattleVisualPrefab; //What will be displayed in battle
    public GameObject MemberOverworldVisualPrefab; //What will be displayed in the overworld scene
}
