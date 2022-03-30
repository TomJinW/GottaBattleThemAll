using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Monster
{
    //fixed state
    [SerializeField] private MonsterBase baseState;
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 10;

    //variable state
    [SerializeField] private int level;
    [SerializeField] private Stats leveledStats;
    
    //battle variable state
    [SerializeField] private int activeHP;
    [SerializeField] private bool isFainted;

    public MonsterBase BaseState { get => baseState; set => baseState = value; }
    public int Level { get => level;}
    public Stats LeveledStats { get => leveledStats;}
    public bool IsFainted { get => isFainted; set => isFainted = value; }

    public void Init()
    {
        level = Mathf.Clamp(Level, MIN_LEVEL, MAX_LEVEL);
        leveledStats = BaseState.BaseStats;

        for(int i = 2;i<=Level;++i)
            levelUp();

        activeHP = LeveledStats.hp;
        IsFainted = false;
    }
    
    #region getters
    public float getNormalizedHP()
    {
        return activeHP / LeveledStats.hp;
    }
    #endregion

    #region modifiers
    public void levelUp()
    {
        if (level >= MAX_LEVEL)
            return;
        
        level++;
        leveledStats += BaseState.StatsChangePerLevel;
    }
    public void takeDamage(int damage)
    {
        activeHP -= damage;
        activeHP = Mathf.Max(activeHP, 0);
        isFainted = activeHP == 0 ? true : false;
    }
    public void revive()
    {
        activeHP = LeveledStats.hp;
        IsFainted = false;
    }
    #endregion
}
