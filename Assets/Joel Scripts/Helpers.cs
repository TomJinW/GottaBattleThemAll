using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct Stats
{
    public int attack, defense, speed, hp;
    public static Stats operator +(Stats a, Stats b)
    {
        a.attack += b.attack;
        a.defense += b.defense;
        a.speed += b.speed;
        a.hp += b.hp;
        return a;
    }
}

public enum Type
{
    Volcano, Swamp, Snowy, None
}

public enum Target
{
    self, ally, singOp, doubOp, all
}
