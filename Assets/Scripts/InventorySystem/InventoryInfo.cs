using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.UI;
using NUnit.Framework.Interfaces;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "InventoryInfo", menuName = "Inventory/New Inventory")]
public class InventoryInfo : ScriptableObject
{
    [field: Space(10)]
    [field: Header("Character Inventory")]
    [field: Space(5)]
    [field: SerializeField] public int MaxInventorySpaces { get; private set; }
    [field: SerializeField] public List<ItemStack> ItemSlots { get; private set; }

    [field: Space(10)]
    [field: Header("Character Equipment")]
    [field: Space(5)]
    [field: SerializeField] public int MaxEquipmentSpaces { get; private set; }
    [field: SerializeField] public int MaxCombatEquipmentAmount { get; private set; }
    [field: SerializeField] public int MaxUtilityEquipmentAmount { get; private set; }
    [field: SerializeField] public int MaxAltMoveEquipmentAmount { get; private set; }

    [field: OnValueChanged("CheckEquipment")]
    [field: SerializeField, Expandable] public List<ItemInfo> EquipmentSlots { get; private set; }
    [field: SerializeField, Expandable] public List<ItemInfo> CombatEquipmentSlots { get; private set; }
    [field: SerializeField, Expandable] public List<ItemInfo> UtilityEquipmentSlots { get; private set; }
    [field: SerializeField, Expandable] public List<ItemInfo> AltMoveEquipmentSlots { get; private set; }

    private void CheckEquipment()
    {
        EquipmentSlots.RemoveAll((e) => e.Type != ItemTypes.Equippable);
    }

    public Inventory Instantiate()
    {
        return new Inventory(this);
    }
}

[System.Serializable]
public class Inventory
{
    public Inventory(InventoryInfo info)
    {
        MaxInventorySpaces = info.MaxInventorySpaces;
        MaxCombatEquipmentAmount = info.MaxCombatEquipmentAmount;
        MaxUtilityEquipmentAmount = info.MaxUtilityEquipmentAmount;
        MaxAltMoveEquipmentAmount = info.MaxAltMoveEquipmentAmount;
        
        ItemSlots = new List<ItemStack>();
        foreach (ItemStack stack in info.ItemSlots)
        {
            ItemSlots.Add(new ItemStack(stack.Item, stack.Amount));
        }

        MaxEquipmentSpaces = info.MaxEquipmentSpaces;
        EquipmentSlots = new List<ItemInfo>(info.EquipmentSlots);
        CombatEquipmentSlots = new List<ItemInfo>(info.CombatEquipmentSlots);
        UtilityEquipmentSlots = new List<ItemInfo>(info.UtilityEquipmentSlots);
        AltMoveEquipmentSlots = new List<ItemInfo>(info.AltMoveEquipmentSlots);

        CheckEquipment();
    }

    public Inventory(int maxInventorySpaces, int maxEquipmentSpaces)
    {
        MaxInventorySpaces = maxInventorySpaces;
        ItemSlots = new List<ItemStack>();

        MaxEquipmentSpaces = maxEquipmentSpaces;
        EquipmentSlots = new List<ItemInfo>();
    }

    [field: Space(10)]
    [field: Header("Character Inventory")]
    [field: Space(5)]
    [field: SerializeField, ReadOnly] public int MaxInventorySpaces { get; private set; }
    [field: SerializeField] public int CombatEquipmentAmount { get; private set; }
    [field: SerializeField] public int UtilityEquipmentAmount { get; private set; }
    [field: SerializeField] public int AltMoveEquipmentAmount { get; private set; }
    [field: SerializeField, ReadOnly] public List<ItemStack> ItemSlots { get; private set; }

    public bool IsFull(bool considerStacks = true)
    {
        if (ItemSlots.Count < MaxInventorySpaces) return false;

        if (!considerStacks) return true;

        foreach (ItemStack stack in ItemSlots)
        {
            if (!stack.IsFull())
            {
                return false;
            }
        }

        return true;
    }

    public bool Contains(ItemInfo item)
    {
        if (ItemSlots.Count == 0) return false;

        foreach (ItemStack stack in ItemSlots)
        {
            if (stack.Item == item) return true;
        }

        return false;
    }

    public bool AddItem(ItemInfo item)
    {
        if (IsFull()) return false;

        foreach (ItemStack stack in ItemSlots)
        {
            if (item == stack.Item)
            {
                if (!stack.IsFull())
                {
                    stack.AddItem();
                    return true;
                }
                else continue;
            }
        }

        if (ItemSlots.Count < MaxInventorySpaces)
            ItemSlots.Add(new ItemStack(item));
        else
            return false;

        return true;
    }

    public void RemoveItem(ItemInfo item)
    {
        if (!Contains(item)) return;

        var reverse = new List<ItemStack>(ItemSlots);
        reverse.Reverse();

        foreach(ItemStack stack in reverse)
        {
            if (stack.Item == item)
            {
                stack.RemoveItem();

                if (stack.Amount <= 0)
                {
                    ItemSlots.Remove(stack);
                }
                return;
            }
        }
    }

    // Remove Item from Specific Stack
    public void RemoveItem(ItemStack stack)
    {
        if (!ItemSlots.Contains(stack)) return;

        stack.RemoveItem();

        if (stack.Amount <= 0)
        {
            ItemSlots.Remove(stack);
        }
    }

    [field: Space(10)]
    [field: Header("Character Equipment")]
    [field: Space(5)]
    [field: SerializeField, ReadOnly] public int MaxEquipmentSpaces { get; private set; }
    [field: SerializeField] public int MaxCombatEquipmentAmount { get; private set; }
    [field: SerializeField] public int MaxUtilityEquipmentAmount { get; private set; }
    [field: SerializeField] public int MaxAltMoveEquipmentAmount { get; private set; }

    [field: OnValueChanged("CheckEquipment")]
    [field: SerializeField, Expandable, ReadOnly] public List<ItemInfo> EquipmentSlots { get; private set; }
    [field: SerializeField, Expandable, ReadOnly] public List<ItemInfo> CombatEquipmentSlots { get; private set; }
    [field: SerializeField, Expandable, ReadOnly] public List<ItemInfo> UtilityEquipmentSlots { get; private set; }
    [field: SerializeField, Expandable, ReadOnly] public List<ItemInfo> AltMoveEquipmentSlots { get; private set; }

    private void CheckEquipment()
    {
        EquipmentSlots.RemoveAll((e) => e.Type != ItemTypes.Equippable);
    }

    public bool EquipmentTypeFull(EquippableTypes equipmentType)
    {   
        bool result = true;

        switch (equipmentType) {
            case EquippableTypes.Combat:
                result = CombatEquipmentSlots.Count >= MaxCombatEquipmentAmount;
                break;
            case EquippableTypes.Utility:
                result = UtilityEquipmentSlots.Count >= MaxUtilityEquipmentAmount;
                break;
            case EquippableTypes.AltMove:
                result = AltMoveEquipmentSlots.Count >= MaxAltMoveEquipmentAmount;
                break;            
        }

        return result;
    }

    public bool HasEquiped(ItemInfo item)
    {
        return EquipmentSlots.Contains(item);
    }

    public bool AddEquipment(ItemInfo item)
    {
        if (item.Type != ItemTypes.Equippable) return false;

        EquippableTypes equipmentType = item.equippableType;

        // If the inventory for the equipment type is full, return
        if (EquipmentTypeFull(equipmentType)) return false;

        // Otherwise add it to its list
        AddEquipmentToTypeList(item, item.equippableType);

        // And add it to the overall list of equipment
        EquipmentSlots.Add(item);

        OnChangeEqupipment?.Invoke();

        return true;
    }

    private void AddEquipmentToTypeList(ItemInfo item, EquippableTypes equipmentType)
    {
        switch (equipmentType) {
            case EquippableTypes.Combat:
                CombatEquipmentSlots.Add(item);
                break;
            case EquippableTypes.Utility:
                UtilityEquipmentSlots.Add(item);
                break;
            case EquippableTypes.AltMove:
                AltMoveEquipmentSlots.Add(item);
                break;            
        }        
    }

    public void RemoveEquipment(ItemInfo item)
    {
        if (item.Type != ItemTypes.Equippable) return;

        // If the inventory doesn't contain the item, return
        if (!EquipmentSlots.Contains(item)) return;

        // Otherwise remove it from its list
        RemoveEquipmentFromTypeList(item, item.equippableType);

        // And from the general items list
        EquipmentSlots.Remove(item);

        OnChangeEqupipment?.Invoke();
    }

    private void RemoveEquipmentFromTypeList(ItemInfo item, EquippableTypes equipmentType)
    {
        switch (equipmentType) {
            case EquippableTypes.Combat:
                CombatEquipmentSlots.Remove(item);
                break;
            case EquippableTypes.Utility:
                UtilityEquipmentSlots.Remove(item);
                break;
            case EquippableTypes.AltMove:
                AltMoveEquipmentSlots.Remove(item);
                break;            
        }        
    }

    public Action OnChangeEqupipment;
}
