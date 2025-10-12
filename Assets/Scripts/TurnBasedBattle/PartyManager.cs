using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Accessibility;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [ReadOnly]
    [SerializeField] private List<PartyMember> currentParty;
    [SerializeField] private PartyMemberInfo defaultPartyMember;
    private Vector3 playerPosition;
    private static GameObject instance;
    private void Awake()
    {
        if (instance != null) //check if the party manager is already included in the scene as when u go abck to the overworld after a battle, the dontdestroyonload() will cause a duplicate
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
            AddMemberToPartyByname(defaultPartyMember.MemberName);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void AddMemberToPartyByname(string memberName)
    {
        for (int i = 0; i < allMembers.Length; i++)
        {
            if (allMembers[i].MemberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.MemberName = allMembers[i].MemberName;
                newPartyMember.Level = allMembers[i].StartingLevel;
                newPartyMember.CurrHealth = allMembers[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                newPartyMember.Strength = allMembers[i].BaseStr;
                newPartyMember.Initiative = allMembers[i].BaseInitiative;
                newPartyMember.MemberBattleVisualPrefab = allMembers[i].MemberBattleVisualPrefab;
                newPartyMember.MemberOverworldVisualPrefab = allMembers[i].MemberOverworldVisualPrefab;

                currentParty.Add(newPartyMember);
            }
        }
    }

    public List<PartyMember> GetCurrentParty()
    {
        List<PartyMember> aliveParty = new List<PartyMember>(); //in case we hae a party with more than one person, only bring the ones who have health != 0
        aliveParty = currentParty; 
        for (int i = 0; i < aliveParty.Count; i++)
        {
            if(aliveParty[i].CurrHealth <= 0)
            {
                aliveParty.RemoveAt(i);
            }
        }
        return aliveParty;
    }

    public void SaveHealth(int partyMember, int health)
    {
        currentParty[partyMember].CurrHealth = health;
    }

    public void SetPosition(Vector3 position)
    {
        playerPosition = position;
    }

    public Vector3 GetPosition()
    {
        return playerPosition;
    }
}

[System.Serializable]
public class PartyMember
{
    public string MemberName;
    public int Level;
    public int CurrHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public int CurrExp;
    public int MaxExp;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;

    public Ability[] CurrentAbilities = new Ability[4]; // The current form-adjusted abilities
    public Item EquippedItem;                      // Currently equipped item (defines ability form)
}