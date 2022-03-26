using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    [Header("Monster State Data")]

    [SerializeField]
    private Type myType;

    [SerializeField]
    private Stats myStats;

    [SerializeField]
    private  Stats statsChangePerLevel;

    [HideInInspector] public  Move[] moveSet;
    private const int MOVESET_SIZE = 4;

    private void OnValidate()
    {
        moveSet = new Move[MOVESET_SIZE];
    }

    #region getters
    public Type getType()
    {
        return myType;
    }

    public Stats getStats()
    {
        return myStats;
    }

    public Stats getStatsChangePerLevel()
    {
        return statsChangePerLevel;
    }

    public Move[] getMoveset()
    {
        return moveSet;
    }

    public Move? getMove(int index) 
    {
        if (index > 4 || index < 0)
            return null;
        
        return moveSet[index];
    }
    #endregion


}


[Serializable]
public struct Stats
{
    public int attack, defense, speed, hp;

    public Stats(int attack, int defense, int speed, int hp)
    {
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.hp = hp;
    }
    public static Stats operator + (Stats a, Stats b)
    {
        a.attack += b.attack;
        a.defense += b.defense;
        a.speed += b.speed;
        a.hp += b.hp;
        return a;
    }
}

[Serializable]
public enum Type
{
    Volcano, Swamp, Snowy
}

[Serializable]
public enum Target
{
    self, ally, singOp, doubOp, all
}

[Serializable]
public struct Move
{
    public String moveName;
    public Type moveType;
    public int baseDamage;
    public Stats statEffects;
    public int critProb;

    public Move(string moveName,Type moveType, int baseDamage, Stats statEffects, int critProb)
    {
        this.moveName = moveName;
        this.moveType = moveType;
        this.baseDamage = baseDamage;
        this.statEffects = statEffects;
        this.critProb = critProb;
    }
}
