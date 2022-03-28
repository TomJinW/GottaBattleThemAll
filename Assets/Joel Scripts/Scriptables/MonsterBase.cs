using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster",menuName = "Persistent/Monster")]
public class MonsterBase : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public Type type;
    public Stats baseStats;
    public Stats statsChangePerLevel;
    public MoveBase[] moves = new MoveBase[4];
}




