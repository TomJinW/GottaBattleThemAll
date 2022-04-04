using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster",menuName = "Persistent/Monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Type type;
    [SerializeField] private Stats baseStats;
    [SerializeField] private Stats statsChangePerLevel;
    [SerializeField] private MoveBase[] moves = new MoveBase[4];
    [SerializeField] private ItemBase[] items = new ItemBase[7];

    public string Name { get => name;}
    public Sprite Sprite { get => sprite;}
    public Type Type { get => type;}
    public Stats BaseStats { get => baseStats;}
    public Stats StatsChangePerLevel { get => statsChangePerLevel;}
    public MoveBase[] Moves { get => moves;}
    public ItemBase[] Items { get => items;}
}




