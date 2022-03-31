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
    [SerializeField] private Stats maxStats;

    //battle variable state
    [SerializeField] private int activeHP;
    [SerializeField] private bool isFainted;

    //public properties
    public MonsterBase BaseState { get => baseState; set => baseState = value; }
    public int Level { get => level; set => level = value; }
    
    //private properties
    public bool IsFainted { get => isFainted;}
    public Stats LeveledStats { get => leveledStats;}
    
    public void Init()
    {
        level = Mathf.Clamp(Level, MIN_LEVEL, MAX_LEVEL);
        
        leveledStats = baseState.BaseStats;
        for (int i = 2; i <= Level; ++i)
            leveledStats += BaseState.StatsChangePerLevel;

        maxStats = baseState.BaseStats;
        for (int i = 2; i <= MAX_LEVEL; ++i)
            maxStats += BaseState.StatsChangePerLevel;

        activeHP = leveledStats.hp;
        isFainted = false;
    }
    
    #region getters
    public float getNormalizedHP()
    {
        return (float)activeHP / LeveledStats.hp;
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
    public void takeTemporaryStatEffects(Stats effects)
    {
        leveledStats += effects;
        leveledStats.attack = Mathf.Clamp(leveledStats.attack,baseState.BaseStats.attack, maxStats.attack);
        leveledStats.defense = Mathf.Clamp(leveledStats.defense, baseState.BaseStats.defense, maxStats.defense);
        leveledStats.speed = Mathf.Clamp(leveledStats.speed, baseState.BaseStats.speed, maxStats.speed);
        leveledStats.hp = Mathf.Clamp(leveledStats.hp, baseState.BaseStats.hp, maxStats.hp);
    }
    public void ResetMonster()
    {
        Init();
    }
    #endregion
}
