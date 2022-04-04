using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Persistent/Items")]
public class ItemBase : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private int quantity;
    [SerializeField] private int index;
    [SerializeField] private ItemType type;
    [SerializeField] private Target target;
    [SerializeField] private Stats statEffects;

    public string Name { get => name;}
    public int Quantity { get => quantity;}
    public int Index { get => index;}
    public ItemType Type { get => type;}
    public Target Target { get => target;}
    public Stats StatEffects { get => statEffects;}
}

